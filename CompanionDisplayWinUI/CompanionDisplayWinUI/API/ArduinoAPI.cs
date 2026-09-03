using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Concurrent;
using System.IO.Ports;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace CompanionDisplayWinUI.API
{
    // These functions only work for an upcoming project which will be published once it's ready
    public class ArduinoAPI
    {
        private ContentControl captureTarget;
        private readonly CanvasDevice captureCanvas = new();
        private int[] targetDimensions = [128, 64];
        private readonly string targetPort;
        private readonly int baudRate = 115200;
        private readonly int opTimeout = 1500;
        private readonly int opCooldown = 2;
        private readonly int chunkSize = 64;
        private CanvasRenderTarget renderTarget;
        private SerialPort serialPort;
        private readonly BlockingCollection<byte[]> frameQueue;
        private int bufferSize;
        private readonly int queueSize = 2;
        private const byte achByte = 0x06;
        readonly DispatcherTimer timer = new() { Interval = TimeSpan.FromMilliseconds(33) };
        public ArduinoAPI(ContentControl captureTarget, string targetPort)
        {
            this.targetPort = targetPort;
            this.captureTarget = captureTarget;
            serialPort = new SerialPort(targetPort, baudRate);
            bufferSize = targetDimensions[0] * targetDimensions[1] / 8;
            frameQueue = new BlockingCollection<byte[]>(queueSize);
            timer.Tick += async (s, args) => await HandleFrame();
            timer.Start();
        }
        public void SetNewSize(int[] newSize)
        {
            targetDimensions = newSize;
            bufferSize = newSize[0] * newSize[1] / 8;
        }
        public void SetCaptureElement(ContentControl newTarget)
        {
            captureTarget = newTarget;
        }
        public void SetFrameRate(int newFPS)
        {
            timer.Stop();
            timer.Interval = TimeSpan.FromMilliseconds(1000 / newFPS);
        }
        private byte[] ResizeFrame(byte[] frameBytes)
        {
            if (renderTarget == null)
            {
                renderTarget = new CanvasRenderTarget(captureCanvas, targetDimensions[0], targetDimensions[1], 96);
            }
            using (CanvasBitmap canvasBitmap = CanvasBitmap.CreateFromBytes(captureCanvas, frameBytes, (int)captureTarget.ActualWidth, (int)captureTarget.ActualHeight, Windows.Graphics.DirectX.DirectXPixelFormat.B8G8R8A8UIntNormalized))
            using (CanvasDrawingSession drawingSession = renderTarget.CreateDrawingSession())
            {
                ScaleEffect scaleEffect = new()
                {
                    Source = canvasBitmap,
                    Scale = new Vector2(targetDimensions[0] / (float)canvasBitmap.Size.Width, targetDimensions[1] / (float)canvasBitmap.Size.Height),
                    InterpolationMode = CanvasImageInterpolation.Linear
                };
                float newWidth = (float)canvasBitmap.Size.Width;
                float newHeight = (float)canvasBitmap.Size.Height;

                drawingSession.Clear(Colors.Black);
                drawingSession.DrawImage(scaleEffect, 0, 0);
            }
            return renderTarget.GetPixelBytes();
        }
        private byte[] GetMonochromeArduinoReady(byte[] frameBytes)
        {
            bool[,] isWhite = new bool[targetDimensions[0], targetDimensions[1]];
            int pixelIndex = 0;
            for (int y = 0; y < targetDimensions[1]; y++)
            {
                for (int x = 0; x < targetDimensions[0]; x++)
                {
                    int i = pixelIndex * 4;
                    if (i + 3 >= frameBytes.Length)
                    {
                        isWhite[x, y] = false;
                        pixelIndex++;
                        continue;
                    }

                    byte b = frameBytes[i];
                    byte g = frameBytes[i + 1];
                    byte r = frameBytes[i + 2];
                    byte a = frameBytes[i + 3];

                    // Un-premultiply if alpha > 0
                    if (a > 0)
                    {
                        float alpha = a / 255.0f;
                        r = (byte)Math.Clamp((int)(r / alpha), 0, 255);
                        g = (byte)Math.Clamp((int)(g / alpha), 0, 255);
                        b = (byte)Math.Clamp((int)(b / alpha), 0, 255);
                    }

                    float l = (0.299f * r + 0.587f * g + 0.114f * b) / 255.0f;
                    isWhite[x, y] = l >= 0.5f;
                    pixelIndex++;
                }
            }
            byte[] oledBuffer = new byte[bufferSize];
            int pages = targetDimensions[1] / 8;
            for (int page = 0; page < pages; page++)
            {
                for (int x = 0; x < targetDimensions[0]; x++)
                {
                    byte columnByte = 0;
                    for (int bit = 0; bit < 8; bit++)
                    {
                        int y = page * 8 + bit;
                        if (y < targetDimensions[1] && isWhite[x, y])
                        {
                            columnByte |= (byte)(1 << bit);
                        }
                    }
                    int dstIndex = x + page * targetDimensions[0];
                    oledBuffer[dstIndex] = columnByte;
                }
            }
            return oledBuffer;
        }
        private void AddFrameToQueue(byte[] frame)
        {
            if (!frameQueue.IsAddingCompleted)
            {
                if (frameQueue.Count >= queueSize)
                {
                    try { frameQueue.TryTake(out _); } catch { }
                }
                frameQueue.TryAdd(frame);
            }
        }
        private async Task HandleFrame()
        {
            try
            {
                RenderTargetBitmap targetBitmap = new();
                await targetBitmap.RenderAsync(captureTarget);
                IBuffer controlPixels = await targetBitmap.GetPixelsAsync();
                byte[] resizedFrame = ResizeFrame(controlPixels.ToArray());
                byte[] arduinoReadyBytes = GetMonochromeArduinoReady(resizedFrame);
                AddFrameToQueue(arduinoReadyBytes);
            }
            catch
            {

            }
        }
        private void InitCOM()
        {
            serialPort = new SerialPort(targetPort, baudRate)
            {
                WriteTimeout = opTimeout,
                ReadTimeout = opTimeout,
            };
        }
        private void SendLoop()
        {
            while (serialPort.IsOpen)
            {
                try
                {
                    byte[] sendFrame = frameQueue.Take();
                    int sent = 0;
                    while (sent < sendFrame.Length)
                    {
                        int data = Math.Min(chunkSize, sendFrame.Length - sent);
                        serialPort.Write(sendFrame, sent, data);
                        sent += data;
                        Thread.Sleep(opCooldown);
                    }
                    int ack = -1;
                    ack = serialPort.ReadByte();
                    if (ack != achByte)
                    {
                        try { serialPort.DiscardInBuffer(); } catch { }
                    }
                } catch { }
            }
        }
        public void ConnectAndStream()
        {
            try
            {
                if (!serialPort.IsOpen)
                {
                    InitCOM();
                    serialPort.Open();
                    Thread thread = new(SendLoop);
                    thread.Start();
                }
            }
            catch { }
        }
        public void DisconnectAndStopStream()
        {
            if (serialPort.IsOpen)
            {
                serialPort.Close();
            }
        }
    }
}
