using CompanionDisplayWinUI.API;
using CompanionDisplayWinUI.ClassImplementations;
using CompanionDisplayWinUI.Objects;
using CoreAudio;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Threading;
using Windows.Media.Control;
using Windows.Storage.Streams;
using Windows.System;

namespace CompanionDisplayWinUI
{
    public sealed partial class MediaPlayerWidget : Page
    {
        public double VolumeCur;
        private SongObject songObject = MusicAPI.currentSong;
        private bool ManualScrolling = false;
        public MediaPlayerWidget()
        {
            this.InitializeComponent();
        }
        private void ChangeSong()
        {
            songObject = MusicAPI.currentSong;
            DispatcherQueue.TryEnqueue(() =>
            {
                LyricsList.Children.Clear();
                try
                {
                    if(songObject.lyricsType == 2)
                    {
                        for (int i = 0; i < songObject.timedLyricsText.Length; i++)
                        {
                            TextBlock textBlock = new()
                            {
                                Text = songObject.timedLyricsText[i]
                            };
                            try
                            {
                                textBlock.Tag = songObject.timedLyricsTimestamps[i];
                                textBlock.Tapped += GoToLyric;
                            }
                            catch
                            {

                            }
                            textBlock.FontSize = 28;
                            textBlock.Opacity = 0.6;
                            textBlock.FontWeight = FontWeights.SemiBold;
                            textBlock.TextWrapping = TextWrapping.WrapWholeWords;
                            LyricsList.Children.Add(textBlock);
                        }
                        LyricsList.Children[0].StartBringIntoView();
                    }
                    else if(songObject.lyricsType == 1)
                    {
                        TextBlock textBlock = new()
                        {
                            Text = songObject.nonTimedLyrics,
                            FontSize = 28,
                            TextWrapping = TextWrapping.WrapWholeWords
                        };
                        LyricsList.Children.Add(textBlock);
                    }
                    else
                    {
                        TextBlock textBlock = new()
                        {
                            Text = AppStrings.mediaNoLyrics,
                            FontSize = 36,
                            TextWrapping = TextWrapping.WrapWholeWords
                        };
                        LyricsList.Children.Add(textBlock);
                    }
                }
                catch { }
            });
        }
        private async void ChangeActiveLyric()
        {
            semaphore.Wait();
            DispatcherQueue.TryEnqueue(() =>
            {
                try
                {
                    if (LyricsList.Children.Count != 0)
                    {
                        for (int i = 0; i < LyricsList.Children.Count; i++)
                        {
                            if (i < MusicAPI.currentLyricIndex)
                            {
                                (LyricsList.Children[i] as TextBlock).Opacity = 0.9;
                            }
                            else if (i == MusicAPI.currentLyricIndex)
                            {
                                (LyricsList.Children[i] as TextBlock).Opacity = 1;
                                if (!ManualScrolling && FlipViewItem.SelectedIndex == 1)
                                {
                                    var options = new BringIntoViewOptions
                                    {
                                        HorizontalAlignmentRatio = 0.5,
                                        VerticalAlignmentRatio = 0.3,
                                        AnimationDesired = true
                                    };
                                    (LyricsList.Children[i] as TextBlock).StartBringIntoView(options);
                                }
                            }
                            else
                            {
                                (LyricsList.Children[i] as TextBlock).Opacity = 0.6;
                            }
                        }
                    }
                }
                catch
                {

                }
            });
            semaphore.Release();
        }
        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            ManualScrolling = true;
        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            ManualScrolling = false;
        }
        private async void GoToLyric(object sender, TappedRoutedEventArgs e)
        {
            await (MusicAPI.currentSession.TryChangePlaybackPositionAsync((long)((double)(sender as TextBlock).Tag) * 10000));
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
            KeyPressAPI.CallKeys(int.Parse((string)(sender as HyperlinkButton).Tag), -1);
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
            MusicAPI.CallInfoUpdate -= ChangeSong;
            MusicAPI.CallLyricUpdate -= ChangeActiveLyric;
            LyricsList.Children.Clear();
            Globals.sleepTimer.CallUpdate -= UpdateIcon;
            MusicAPI.CallInfoUpdate -= UpdateUI;
            MusicAPI.CallCoverUpdate -= UpdateCover;
            MusicAPI.CallTimingUpdate -= TimingUpdate;
            MusicAPI.CallLyricUpdate -= LyricsUpdate;
        }
        private static readonly SemaphoreSlim semaphore = new(1);
        private void AnimateSideways(TranslateTransform translateTransform, TextBlock target)
        {
            double containerWidth = SongStack.ActualWidth;
            double textWidth = target.ActualWidth;
            double maxOffset = textWidth - containerWidth;
            Storyboard sb0 = target.Tag as Storyboard;
            if(sb0 == null)
            {
                sb0 = new Storyboard();
                target.Tag = sb0;
                var anim = new DoubleAnimation
                {
                    From = 0,
                    To = -maxOffset,
                    Duration = TimeSpan.FromSeconds(5),
                    AutoReverse = true,
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut },
                    RepeatBehavior = RepeatBehavior.Forever,
                };
                Storyboard.SetTarget(anim, translateTransform);
                Storyboard.SetTargetProperty(anim, "X");
                sb0.Children.Add(anim);
                sb0.Begin();
            }
            DoubleAnimation doubleAnimation = sb0.Children[0] as DoubleAnimation;
            if (textWidth > 460)
            {
                doubleAnimation.To = -maxOffset;
                sb0.Begin();
            }
            else
            {
                sb0.Stop();
            }
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
            MusicAPI.CallInfoUpdate += UpdateUI;
            MusicAPI.CallTimingUpdate += TimingUpdate;
            MusicAPI.CallLyricUpdate += LyricsUpdate;
            DispatcherQueue.TryEnqueue(() =>
            {
                UpdateUI();
                TimingUpdate();
            });
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
            MusicAPI.CallInfoUpdate += UpdateUI;
            MusicAPI.CallTimingUpdate += TimingUpdate;
            MusicAPI.CallLyricUpdate += LyricsUpdate;
            MusicAPI.CallInfoUpdate += ChangeSong;
            MusicAPI.CallLyricUpdate += ChangeActiveLyric;
            MusicAPI.CallCoverUpdate += UpdateCover;
            ChangeSong();
            ChangeActiveLyric();
            Thread thread0 = new(StartUI);
            thread0.Start();
            UpdateIcon();
        }
        private async void Grid_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            try
            {
                long maxpos = long.Parse(MusicAPI.timelineProperties.EndTime.Ticks.ToString());
                long newpos = (long)(Math.Round((SongProgressBar.Value / 100) * maxpos));
                await(MusicAPI.currentSession.TryChangePlaybackPositionAsync(newpos));
                SongProgressBar.IsFocusEngaged = false;
            }
            catch { }
            IsDragging = false;
        }

        private async void SongProgressBar_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                long maxpos = long.Parse(MusicAPI.timelineProperties.EndTime.Ticks.ToString());
                long newpos = (long)(Math.Round((SongProgressBar.Value / 100) * maxpos));
                await(MusicAPI.currentSession.TryChangePlaybackPositionAsync(newpos));
                IsDragging = false;
                SongProgressBar.IsFocusEngaged = false;
            }
            catch { }
        }

        private void Grid_PointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            SongProgressBar_Tapped(sender, null);
        }
        private void SleepTimer_Click(object sender, RoutedEventArgs e)
        {
            PopupAPI.OpenSleepDialogue(this.XamlRoot);
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
                try
                {
                    SongTitle.Text = songObject.title;
                    AlbumName.Text = songObject.album;
                    SongInfo.Text = MusicAPI.BuildDetails();
                }
                catch { }
            });
        }
        private void UpdateCover()
        {
            AlbumCoverImg.Source = songObject.albumCover;
        }
        private void TimingUpdate()
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                CurrentTime.Text = MusicAPI.songElapsedFormatted;
                EndTime.Text = MusicAPI.songEndFormatted;
                if (!IsDragging)
                {
                    try
                    {
                        SongProgressBar.Value = MusicAPI.songProgress;
                    }
                    catch
                    {
                        SongProgressBar.Value = 0;
                    }
                }
                try
                {
                    if (MusicAPI.playbackInfo.PlaybackStatus == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing)
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

        private async void SongProgressBar_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            await semaphore.WaitAsync();
            AnimateSideways((sender as TextBlock).RenderTransform as TranslateTransform, sender as TextBlock);
            semaphore.Release();
        }

        private void SongStack_Loaded(object sender, RoutedEventArgs e)
        {
            FrameworkElement frameworkElement = sender as FrameworkElement;
            FancyPantsUX.SetupMaskedContainer(frameworkElement.Parent as FrameworkElement, frameworkElement);
        }

        private void LyricsUpdate()
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                if(LyricsList.Children.Count == 1)
                {
                    ChangeSong();
                }
                SongLyrics.Text = MusicAPI.currentLyric;
            });
        }
    }
}
