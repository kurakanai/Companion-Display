using CompanionDisplayWinUI.ClassImplementations;
using LibreHardwareMonitor.Hardware;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading;

namespace CompanionDisplayWinUI.API
{
    internal class HardwareMonitorAPI
    {
        public class UpdateVisitor : IVisitor
        {
            public void VisitComputer(IComputer computer)
            {
                computer.Traverse(this);
            }
            public void VisitHardware(IHardware hardware)
            {
                hardware.Update();
                foreach (IHardware subHardware in hardware.SubHardware) subHardware.Accept(this);
            }
            public void VisitSensor(ISensor sensor) { }
            public void VisitParameter(IParameter parameter) { }
        }
        public Computer computer = new()
        {
            IsCpuEnabled = true,
            IsGpuEnabled = true,
            IsMemoryEnabled = true,
            IsMotherboardEnabled = true,
            IsControllerEnabled = true,
            IsNetworkEnabled = true,
            IsStorageEnabled = true
        };
        public void Init()
        {
            try
            {
                computer.Open();
                computer.Accept(new UpdateVisitor());
                Thread thread = new(UpdateSensor);
                thread.Start();
            }
            catch { }
        }
        public static event CommonlyAccessedInstances.HandleEventsWithNoArgs UpdateSensorValueEvent;
        static void CallSensorUpdate()
        {
            UpdateSensorValueEvent?.Invoke();
        }
        public void UpdateSensor() 
        {
            if(Globals.CurrentHW != null)
            {
                UpdateVisitor update = new();
                update.VisitHardware(Globals.CurrentHW);
            }
            CallSensorUpdate();
            Thread.Sleep(5000);
            Thread thread = new(UpdateSensor);
            thread.Start();
        }
        public static void UpdateSensorValue(ISensor sensor, double lastValue, TextBlock textBlock, ProgressRing ring, string termination, DispatcherQueue dispatcher, bool extraPrecision)
        {
            if (sensor == null) return;
            double? rawValue = sensor.Value;
            if (!rawValue.HasValue) return;
            int decimals = extraPrecision ? 2 : 0;
            double value = Math.Round(rawValue.Value, decimals);
            if (value == lastValue || lastValue == -2) return;
            var currentHW = Globals.CurrentHW;
            if (currentHW != sensor.Hardware && currentHW != sensor.Hardware?.Parent) return;
            double? maxVal = sensor.Max;
            string displayText = $"{value}{termination}";
            dispatcher.TryEnqueue(() =>
            {
                textBlock.Text = displayText;

                if (ring != null)
                {
                    ring.Value = value;
                    if (maxVal.HasValue && Math.Abs(ring.Maximum - maxVal.Value) > 0.001)
                    {
                        ring.Maximum = maxVal.Value;
                    }
                }
            });
        }
    }
}
