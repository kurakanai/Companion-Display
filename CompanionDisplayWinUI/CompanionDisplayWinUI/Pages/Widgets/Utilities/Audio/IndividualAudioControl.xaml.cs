using CompanionDisplayWinUI.ClassImplementations;
using CoreAudio;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class IndividualAudioControl : Page
    {
        public IndividualAudioControl()
        {
            this.InitializeComponent();
        }
        private AudioSessionControl2 audio;
        private MMDevice mDevice;
        private bool IsGlobal = false, IsStarted = false;
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (!IsStarted)
            {
                if ((this.Parent as Frame).Name == "GLOBAL")
                {
                    try
                    {
                        mDevice = (this.Parent as Frame).Tag as MMDevice;
                        mDevice.AudioEndpointVolume.OnVolumeNotification += UpdateVolStuff;
                        Volume.Value = mDevice.AudioEndpointVolume.MasterVolumeLevelScalar * 100;
                        SysIcon.Visibility = Visibility.Visible;
                        ProcessName.Text = AppStrings.volumeMaster;
                        IsGlobal = true;
                    }
                    catch { }
                }
                else
                {
                    try
                    {
                        audio = (this.Parent as Frame).Tag as AudioSessionControl2;
                        audio.OnStateChanged += RemoveSesh;
                        audio.OnSimpleVolumeChanged += VolumeChanged;
                        Volume.Value = audio.SimpleAudioVolume.MasterVolume * 100;
                        if (audio.IsSystemSoundsSession)
                        {
                            ProcessName.Text = AppStrings.volumeSystem;
                            SysIcon.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            using (var p = Process.GetProcessById((int)audio.GetProcessID)) { ProcessName.Text = p.ProcessName; }
                            SysIcon.Visibility = Visibility.Collapsed;
                        }
                        using var process = Process.GetProcessById((int)audio.GetProcessID);
                        using Icon icon = System.Drawing.Icon.ExtractAssociatedIcon(process.MainModule.FileName);
                        using Bitmap bitmap = icon.ToBitmap();
                        using MemoryStream iconStream = new();
                        bitmap.Save(iconStream, ImageFormat.Png);
                        iconStream.Seek(0, SeekOrigin.Begin);
                        BitmapImage bitmapImage = new();
                        bitmapImage.SetSource(iconStream.AsRandomAccessStream());
                        ProcessImage.Source = bitmapImage;
                    }
                    catch
                    {

                    }
                }
                IsStarted = true;
            }
        }

        private void UpdateVolStuff(AudioVolumeNotificationData data)
        {
            if (!IsManipulative)
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    Volume.Value = data.MasterVolume * 100;
                    if (data.Muted)
                    {
                        SysIcon.Symbol = Symbol.Mute;
                    }
                    else
                    {
                        SysIcon.Symbol = Symbol.Volume;
                    }
                });
            }
        }
        private void VolumeChanged(object sender, float newVolume, bool newMute)
        {
            if (!IsManipulative)
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    Volume.Value = newVolume * 100;
                    if (newMute)
                    {
                        if (!audio.IsSystemSoundsSession)
                        {
                            SysIcon.Visibility = Visibility.Visible;
                            ProcessImage.Visibility = Visibility.Collapsed;
                        }
                        SysIcon.Symbol = Symbol.Mute;
                    }
                    else
                    {
                        if (!audio.IsSystemSoundsSession)
                        {
                            SysIcon.Visibility = Visibility.Collapsed;
                            ProcessImage.Visibility = Visibility.Visible;
                        }
                        SysIcon.Symbol = Symbol.Volume;
                    }
                });
            }
        }
        private void RemoveSesh(object sender, AudioSessionState state)
        {
            if (state == AudioSessionState.AudioSessionStateExpired)
            {
                audio.OnStateChanged -= RemoveSesh;
                audio.Dispose();
                DispatcherQueue.TryEnqueue(() =>
                {
                    try
                    {
                        (((this as IndividualAudioControl).Parent as Frame).Parent as StackPanel).Children.Remove((this as IndividualAudioControl).Parent as Frame);
                    }
                    catch { }
                });
            }
        }
        private bool IsManipulative = false;
        private void Volume_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (IsStarted && IsManipulative)
            {
                if (IsGlobal)
                {
                    mDevice.AudioEndpointVolume.MasterVolumeLevelScalar = (float)(Volume.Value / 100);
                }
                else
                {
                    audio.SimpleAudioVolume.MasterVolume = (float)(Volume.Value / 100);
                }
            }
        }

        private void Volume_ManipulationStarting(object sender, ManipulationStartingRoutedEventArgs e)
        {
            IsManipulative = true;
        }

        private void Volume_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            IsManipulative = false;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                mDevice.AudioEndpointVolume.OnVolumeNotification -= UpdateVolStuff;
                audio.OnStateChanged -= RemoveSesh;
                audio.OnSimpleVolumeChanged -= VolumeChanged;
            }
            catch { }
        }

        private void SysIcon_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if(ProcessName.Text == AppStrings.volumeMaster)
            {
                mDevice.AudioEndpointVolume.Mute = !mDevice.AudioEndpointVolume.Mute;
                if (mDevice.AudioEndpointVolume.Mute)
                {
                    SysIcon.Symbol = Symbol.Volume;
                }
                else
                {
                    SysIcon.Symbol = Symbol.Mute;
                }
            }
            else
            {
                audio.SimpleAudioVolume.Mute = !audio.SimpleAudioVolume.Mute;
                if (!audio.SimpleAudioVolume.Mute)
                {
                    if (!audio.IsSystemSoundsSession)
                    {
                        SysIcon.Visibility = Visibility.Visible;
                        ProcessImage.Visibility = Visibility.Collapsed;
                    }
                    SysIcon.Symbol = Symbol.Mute;
                }
                else
                {
                    if (!audio.IsSystemSoundsSession)
                    {
                        SysIcon.Visibility = Visibility.Collapsed;
                        ProcessImage.Visibility = Visibility.Visible;
                    }
                    SysIcon.Symbol = Symbol.Volume;
                }
            }
        }
    }
}
