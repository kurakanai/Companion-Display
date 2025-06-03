using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Threading;
using Windows.Media.Control;
using Windows.Storage.Streams;
using CoreAudio;
using Windows.System;
using System.Runtime.InteropServices;
using CompanionDisplayWinUI.ClassImplementations;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MediaPlayerWidget : Page
    {
        public double VolumeCur;
        public MediaPlayerWidget()
        {
            this.InitializeComponent();
        }
        public bool CleanUp = false, IsDragging = false;
        internal static class Helper
        {
            internal static BitmapImage GetThumbnail(IRandomAccessStreamReference Thumbnail)
            {
                if (Thumbnail == null)
                {
                    return null;
                }
                using IRandomAccessStreamWithContentType imageStream = Thumbnail.OpenReadAsync().GetAwaiter().GetResult();
                using DataReader reader = new(imageStream);
                using var stream = new InMemoryRandomAccessStream();
                using var writer = new DataWriter(stream);
                byte[] fileBytes = new byte[imageStream.Size];
                reader.LoadAsync((uint)imageStream.Size).GetAwaiter().GetResult();
                reader.ReadBytes(fileBytes);
                BitmapImage image = new();
                writer.WriteBytes(fileBytes);
                _ = writer.StoreAsync();
                _ = writer.FlushAsync();
                writer.DetachStream();
                stream.Seek(0);
                _ = image.SetSourceAsync(stream);
                return image;
            }
        }
        private void PressKey(object sender, RoutedEventArgs e)
        {
            KeyPressAPI.callKeys(int.Parse((string)(sender as HyperlinkButton).Tag), -1);
        }
        private void VolumeBar_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (IsManipulative)
            {
                mDevice.AudioEndpointVolume.MasterVolumeLevelScalar = (float)(VolumeBar.Value / 100);
            }
        }
        private MMDevice mDevice;
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            CleanUp = true;
            Globals.sleepTimer.CallUpdate -= UpdateIcon;
            Media.CallInfoUpdate -= UpdateUI;
            Media.CallTimingUpdate -= TimingUpdate;
            Media.CallLyricUpdate -= LyricsUpdate;
        }

        private void UpdateIcon()
        {
            if (Globals.sleepTimer.isEnabled)
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    SleepTimer.Content = "\uf0ce";
                });
            }
            else
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    SleepTimer.Content = "\ue708";
                });
            }
        }

        private void StartUI()
        {
            MMDeviceEnumerator DevEnum = new();
            mDevice = DevEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
            mDevice.AudioEndpointVolume.OnVolumeNotification += ChangeVol;
            VolumeCur = mDevice.AudioEndpointVolume.MasterVolumeLevelScalar * 100;
            DispatcherQueue.TryEnqueue(() =>
            {
                VolumeBar.Value = VolumeCur;
            });
            Media.CallInfoUpdate += UpdateUI;
            Media.CallTimingUpdate += TimingUpdate;
            Media.CallLyricUpdate += LyricsUpdate;
            UpdateUI();
            TimingUpdate();
        }
        private bool IsManipulative = false;
        private void ChangeVol(AudioVolumeNotificationData data)
        {
            if (!IsManipulative)
            {
                try
                {
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        VolumeBar.Value = data.MasterVolume * 100;
                    });
                }
                catch { }
            }
        }

        private void SongProgressBar_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            IsDragging = true;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CleanUp = false;
            MMDeviceEnumerator DevEnum = new();
            mDevice = DevEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
            Globals.sleepTimer.CallUpdate += UpdateIcon;
            Media.CallInfoUpdate += UpdateUI;
            Media.CallTimingUpdate += TimingUpdate;
            Media.CallLyricUpdate += LyricsUpdate;
            Thread thread0 = new(StartUI);
            thread0.Start();
            UpdateIcon();
        }
        private async void Grid_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            try
            {
                long maxpos = long.Parse(Globals.sessionManager.GetCurrentSession().GetTimelineProperties().EndTime.Ticks.ToString());
                long newpos = (long)(Math.Round((SongProgressBar.Value / 100) * maxpos));
                await(Globals.sessionManager.GetCurrentSession().TryChangePlaybackPositionAsync(newpos));
                IsDragging = false;
                SongProgressBar.IsFocusEngaged = false;
            }
            catch
            {
                IsDragging = false;
            }
        }

        private async void SongProgressBar_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                long maxpos = long.Parse(Globals.sessionManager.GetCurrentSession().GetTimelineProperties().EndTime.Ticks.ToString());
                long newpos = (long)(Math.Round((SongProgressBar.Value / 100) * maxpos));
                await(Globals.sessionManager.GetCurrentSession().TryChangePlaybackPositionAsync(newpos));
                IsDragging = false;
                SongProgressBar.IsFocusEngaged = false;
            }
            catch
            {
                IsDragging = false;
            }
        }

        private async void Grid_PointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            SongProgressBar_Tapped(sender, null);
        }
        private void SleepTimer_Click(object sender, RoutedEventArgs e)
        {
            Media.OpenSleepDialogue(this.XamlRoot);
        }

        private void HyperlinkButton_Tapped_1(object sender, RoutedEventArgs e)
        {
            if(VolumeBar.Value <= 98)
            {
                mDevice.AudioEndpointVolume.MasterVolumeLevelScalar = (float)((VolumeBar.Value + 2) / 100);
            }
            else
            {
                mDevice.AudioEndpointVolume.MasterVolumeLevelScalar = 1;
            }
        }

        private void HyperlinkButton_Tapped(object sender, RoutedEventArgs e)
        {
            if (VolumeBar.Value >= 2)
            {
                mDevice.AudioEndpointVolume.MasterVolumeLevelScalar = (float)((VolumeBar.Value - 2) / 100);
            }
            else
            {
                mDevice.AudioEndpointVolume.MasterVolumeLevelScalar = 0;
            }
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            OpenWindow.IsEnabled = false;
            LyricsView m_window = new();
            m_window.Closed += (s, e) =>
            {
                OpenWindow.IsEnabled = true;
            };
            m_window.Activate();
        }

        private void VolumeBar_ManipulationStarting(object sender, ManipulationStartingRoutedEventArgs e)
        {
            IsManipulative = true;
        }

        private void VolumeBar_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            IsManipulative = false;
        }

        private void UpdateUI()
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                SongTitle.Text = Media.SongName;
                AlbumName.Text = Media.AlbumName;
                SongInfo.Text = Media.SongDetails;
                Media.GetCover(DispatcherQueue, AlbumCoverImg);
                AlbumCoverImg.Opacity = 0.2;
            });
        }
        private void TimingUpdate()
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                CurrentTime.Text = Media.SongTime;
                EndTime.Text = Media.SongEnd;
                if (!IsDragging)
                {
                    try
                    {
                        SongProgressBar.Value = Media.SongProgress;
                    }
                    catch
                    {
                        SongProgressBar.Value = 0;
                    }
                }
                try
                {
                    if (Globals.sessionManager.GetCurrentSession().GetPlaybackInfo().PlaybackStatus == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing)
                    {
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            PlayPauseBtn.Content = "\uf8ae";
                        });
                    }
                    else
                    {
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            PlayPauseBtn.Content = "\uf5b0";
                        });
                    }
                }
                catch
                {
                }
            });
        }
        private void LyricsUpdate()
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                SongLyrics.Text = Media.SongLyrics;
            });
        }
    }
}
