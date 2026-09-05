using CompanionDisplayWinUI.API;
using CoreAudio;
using System;
using System.Threading;
using System.Timers;

namespace CompanionDisplayWinUI.ClassImplementations
{
    internal class SleepTimer
    {
        public delegate void HandleSleepModeStartStop();
        public event HandleSleepModeStartStop CallUpdate;
        public bool isEnabled = false;
        private System.Timers.Timer timer = new();
        private readonly MMDeviceEnumerator DevEnum = new();
        private MMDevice device;
        private float initialVolume = 0;
        public void StartTimer(int duration)
        {
            if(duration == 0)
            {
                duration = 300000;
            }
            else
            {
                duration *= 60000;
            }
            timer = new System.Timers.Timer(duration);
            timer.Elapsed += TimerEnded;
            timer.Start();
            isEnabled = true;
            OnUpdate();
        }
        
        public void CancelTimer()
        {
            timer.Stop();
            timer.Dispose();
            isEnabled = false;
            OnUpdate();
        }
        private void TimerEnded(object sender, ElapsedEventArgs e)
        {
            timer.Dispose();
            timer = new System.Timers.Timer(5000)
            {
                AutoReset = true
            };
            timer.Elapsed += VolumeDownCycle;
            device = DevEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
            initialVolume = device.AudioEndpointVolume.MasterVolumeLevelScalar;
            timer.Start();
        }
        private async void VolumeDownCycle(object sender, ElapsedEventArgs e)
        {
            if(device.AudioEndpointVolume.MasterVolumeLevelScalar == 0)
            {
                timer.Stop();
                timer.Dispose();
                await MusicAPI.currentSession.TryPauseAsync();
                Thread.Sleep(300);
                device.AudioEndpointVolume.MasterVolumeLevelScalar = initialVolume;
                isEnabled = false;
                OnUpdate();
            }
            else
            {
                device.AudioEndpointVolume.MasterVolumeLevelScalar = MathF.Max(0, (float)(device.AudioEndpointVolume.MasterVolumeLevelScalar - 0.02));
            }
        }
        public void OnUpdate()
        {
            CallUpdate?.Invoke();
        }
    }
}
