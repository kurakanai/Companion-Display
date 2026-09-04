using OBSWebsocketDotNet;
using OBSWebsocketDotNet.Communication;
using OBSWebsocketDotNet.Types;
using OBSWebsocketDotNet.Types.Events;
using System;
using System.Threading;

namespace CompanionDisplayWinUI
{
    internal class ObsControls
    {
        public OBSWebsocket obs = new()
        {
            WSTimeout = TimeSpan.FromSeconds(1),
        };
        public bool connectionSuccessful = false;
        public bool initialized = false;
        public delegate void HandleStupidEvents();
        public delegate void HandleStupidEvents2();
        public event HandleStupidEvents CallUpdate;
        public event HandleStupidEvents2 CallUpdateStats;
        public void Connect()
        {
            try
            {
                obs.Connected += Connectionstep2;
                obs.Disconnected += Disconnect;
                obs.ConnectAsync(Globals.obsIP, Globals.obsPass);
            }
            catch { }
        }
        private void ReqStats()
        {
            while (connectionSuccessful)
            {
                try
                {
                    obsData = obs.GetStats();
                    outputStatus = obs.GetStreamStatus();
                    recStatus = obs.GetRecordStatus();
                    OnUpdate();
                }
                catch
                {

                }
                Thread.Sleep(1000);
            }
        }
        private void Disconnect(object sender, ObsDisconnectionInfo e)
        {
            connectionSuccessful = false;
        }
        protected virtual void OnUpdate()
        {
            CallUpdateStats?.Invoke(); // Pass EventArgs.Empty for no data
        }
        protected virtual void OnMyEvent()
        {
            CallUpdate?.Invoke(); // Pass EventArgs.Empty for no data
        }
        private void Connectionstep2(object sender, EventArgs e)
        {
            obs.CurrentProgramSceneChanged += UpdateSelection;
            Thread thread = new(ReqStats);
            thread.Start();
            connectionSuccessful = true;
            UpdateSelection(sender, null);
        }

        private void UpdateSelection(object sender, ProgramSceneChangedEventArgs e)
        {
            try
            {
                currentSession = obs.GetCurrentProgramScene();
                scenes = [.. obs.ListScenes()];
                OnMyEvent();
            }
            catch
            {
                connectionSuccessful = false;
            }
        }
        public ObsStats obsData = new();
        public OutputStatus outputStatus = new();
        public RecordingStatus recStatus = new();
        public void ManualConnectReq()
        {
            try
            {
                obs.ConnectAsync(Globals.obsIP, Globals.obsPass);
            }
            catch
            {

            }
        }
        public SceneBasicInfo[] scenes;
        public string currentSession = "";
        public void SetScene(string newScene)
        {
            obs.SetCurrentProgramScene(newScene);
        }
        public void StartStreaming()
        {
            obs.StartStream();
        }
        public void StartRecording()
        {
            obs.StartRecord();
        }
        public void StopStreaming()
        {
            obs.StopStream();
        }
        public void StopRecording()
        {
            obs.StopRecord();
        }
        public void PauseToggle()
        {
            obs.ToggleRecordPause();
        }
        public void CameraToggle()
        {
            obs.ToggleVirtualCam();
        }
        public void BufferToggle()
        {
            obs.ToggleReplayBuffer();
        }
        public void BufferSave()
        {
            obs.SaveReplayBuffer();
        }
        public bool micMute = false;
        public void MicToggle()
        {
            foreach(var input in obs.GetInputList())
            {
                try
                {
                    obs.ToggleInputMute(input.InputName);
                }
                catch
                {

                }
                micMute = !micMute;
            }
        }
        public void Reconnect()
        {
            obs.Disconnect();
            Connect();
        }
    }
}
