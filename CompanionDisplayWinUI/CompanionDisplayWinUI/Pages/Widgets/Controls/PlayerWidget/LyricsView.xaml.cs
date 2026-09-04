using CompanionDisplayWinUI.API;
using CompanionDisplayWinUI.ClassImplementations;
using CompanionDisplayWinUI.Objects;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System;
using System.Runtime.InteropServices;
using Windows.Media.Control;
using Windows.System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LyricsView : Window
    {
        public LyricsView()
        {
            this.InitializeComponent();
            this.AppWindow.Resize(new Windows.Graphics.SizeInt32 { Width = 800, Height = 450 });
            this.ExtendsContentIntoTitleBar = true;
        }

        // Constants for Window Styles
        private static readonly IntPtr HWND_TOPMOST = new(-1);
        private static readonly IntPtr HWND_NOTOPMOST = new(-2);
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_NOSIZE = 0x0001;
        private SongObject songObject = MusicAPI.currentSong;

        // Import SetWindowLong and GetWindowLong from User32.dll
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(
            IntPtr hWnd,
            IntPtr hWndInsertAfter,
            int X,
            int Y,
            int cx,
            int cy,
            uint uFlags);
        private void ToggleButton1_Checked(object sender, RoutedEventArgs e)
        {
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            SetWindowPos(hwnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
        }

        private void ToggleButton1_Unchecked(object sender, RoutedEventArgs e)
        {
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            SetWindowPos(hwnd, HWND_NOTOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
        }
        private void PlayPauseBtn_Click(object sender, RoutedEventArgs e)
        {
            KeyPressAPI.CallKeys(int.Parse((string)(sender as HyperlinkButton).Tag), -1);
        }
        public bool CleanUp = false, IsDragging = false;
        private async void SongProgressBar_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                long maxpos = long.Parse(MusicAPI.timelineProperties.EndTime.Ticks.ToString());
                long newpos = (long)(Math.Round((SongProgressBar.Value / 100) * maxpos));
                await (MusicAPI.currentSession.TryChangePlaybackPositionAsync(newpos));
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
            try
            {
                long maxpos = long.Parse(MusicAPI.timelineProperties.EndTime.Ticks.ToString());
                long newpos = (long)(Math.Round((SongProgressBar.Value / 100) * maxpos));
                await (MusicAPI.currentSession.TryChangePlaybackPositionAsync(newpos));
                IsDragging = false;
                SongProgressBar.IsFocusEngaged = false;
            }
            catch
            {
                IsDragging = false;
            }
        }


        private void SongProgressBar_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            IsDragging = true;
        }

        private async void Grid_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            try
            {
                long maxpos = long.Parse(MusicAPI.timelineProperties.EndTime.Ticks.ToString());
                long newpos = (long)(Math.Round((SongProgressBar.Value / 100) * maxpos));
                await (MusicAPI.currentSession.TryChangePlaybackPositionAsync(newpos));
                IsDragging = false;
                SongProgressBar.IsFocusEngaged = false;
            }
            catch
            {
                IsDragging = false;
            }
        }
        private void ChangeSong()
        {
            songObject = MusicAPI.currentSong;
            DispatcherQueue.TryEnqueue(() =>
            {
                LyricsList.Children.Clear();
                try
                {
                    this.Title = songObject.artist + " · " + MusicAPI.BuildDetails();
                    titleSong.Text = songObject.title;
                    detailsSong.Text = MusicAPI.BuildDetails();
                    EndTime.Text = MusicAPI.songEndFormatted;
                    AlbumCoverImg.Source = songObject.albumCover;
                    BackgroundImage.Source = AlbumCoverImg.Source;
                    if (songObject.lyricsType == 2)
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
                    else if (songObject.lyricsType == 1)
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
        private void ChangeTime()
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                try
                {
                    CurrentTime.Text = MusicAPI.songElapsedFormatted;
                    SongProgressBar.Value = MusicAPI.songProgress;
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
        private void ChangeActiveLyric()
        {
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
                                if (!ManualScrolling)
                                {
                                    var options = new BringIntoViewOptions
                                    {
                                        HorizontalAlignmentRatio = 0.5, // Center horizontally
                                        VerticalAlignmentRatio = 0.5,   // Center vertically
                                        AnimationDesired = true         // Optional: Smooth scrolling
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
        }
        private async void GoToLyric(object sender, TappedRoutedEventArgs e)
        {
            await (MusicAPI.currentSession.TryChangePlaybackPositionAsync((long)((double)(sender as TextBlock).Tag) * 10000));
        }

        private bool ManualScrolling = false;
        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            ManualScrolling = true;
        }

        private void BackgroundImage_ImageOpened(object sender, RoutedEventArgs e)
        {
            ImageOptionalBlur.Stretch = Stretch.None;
            ImageOptionalBlur.Stretch = Stretch.UniformToFill;
        }
        private void Window_Closed(object sender, WindowEventArgs args)
        {
            Globals.sleepTimer.CallUpdate -= UpdateIcon;
            MusicAPI.CallInfoUpdate -= ChangeSong;
            MusicAPI.CallTimingUpdate -= ChangeTime;
            MusicAPI.CallLyricUpdate -= ChangeActiveLyric;
            LyricsList.Children.Clear();
        }
        private void UpdateIcon()
        {
            if (Globals.sleepTimer.isEnabled)
            {
                SleepTimer.Content = "\uf0ce";
            }
            else
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    SleepTimer.Content = "\ue708";
                });
            }
        }
        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            Globals.sleepTimer.CallUpdate += UpdateIcon;
            MusicAPI.CallInfoUpdate += ChangeSong;
            MusicAPI.CallTimingUpdate += ChangeTime;
            MusicAPI.CallLyricUpdate += ChangeActiveLyric;
            UpdateIcon();
            ChangeSong();
            ChangeTime();
            ChangeActiveLyric();
        }

        private void SleepTimer_Click(object sender, RoutedEventArgs e)
        {
            PopupAPI.OpenSleepDialogue(MainGrid.XamlRoot);
        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            ManualScrolling = false;
        }
    }
}
