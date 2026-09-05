using CompanionDisplayWinUI.API;
using CompanionDisplayWinUI.ClassImplementations;
using CompanionDisplayWinUI.Objects;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Threading;
using Windows.Media.Control;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SleepPage : Page
    {
        public SleepPage()
        {
            this.InitializeComponent();
            this.DataContext = Globals.publicTimeViewModel;
        }
        private bool CleanUp = false;
        SongObject songObject = MusicAPI.currentSong;
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            CleanUp = true;
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (Globals.OverrideColor)
            {
                try
                {
                    CleanUp = false;
                    Time.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, (byte)Globals.SleepColorR, (byte)Globals.SleepColorG, (byte)Globals.SleepColorB));
                    Date.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, (byte)Globals.SleepColorR, (byte)(byte)Globals.SleepColorG, (byte)Globals.SleepColorB));
                    SongTitle.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, (byte)Globals.SleepColorR, (byte)Globals.SleepColorG, (byte)Globals.SleepColorB));
                    Lyrics.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, (byte)Globals.SleepColorR, (byte)Globals.SleepColorG, (byte)Globals.SleepColorB));
                }
                catch
                {

                }
            }
            StackUnderflow.Opacity = Globals.sleepModeOpacity / 100;
            Oppenheimer.Opacity = StackUnderflow.Opacity;
            Thread thread = new(UpdateUI);
            thread.Start();
            try
            {
                (((this.Parent) as Frame).Parent as NavigationView).IsPaneToggleButtonVisible = false;
                (((this.Parent) as Frame).Parent as NavigationView).PaneDisplayMode = NavigationViewPaneDisplayMode.LeftMinimal;
            }
            catch
            {

            }
            if (!Globals.StartedPlayer)
            {
                Globals.StartedPlayer = !Globals.StartedPlayer;
            }
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            (((this.Parent) as Frame).Parent as NavigationView).IsPaneOpen = true;
        }
        private void UpdateUI()
        {
            try
            {
                songObject = MusicAPI.currentSong;
                DispatcherQueue.TryEnqueue(() =>
                {
                    try
                    {
                        bool showMedia = MusicAPI.playbackInfo != null && MusicAPI.playbackInfo.PlaybackStatus == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing;
                        if (showMedia)
                        {
                            SongTitle.Text = songObject.title + " · " + MusicAPI.BuildDetails();
                            Lyrics.Text = MusicAPI.currentLyric;
                        }
                        SongTitle.Visibility = (Visibility)Convert.ToSByte(!showMedia);
                        Lyrics.Visibility = (Visibility)Convert.ToSByte(MusicAPI.currentLyric == "" || !showMedia || !(SongTitle.Visibility == Visibility.Visible));
                    }
                    catch
                    {
                        SongTitle.Visibility = Visibility.Collapsed;
                        Lyrics.Visibility = Visibility.Collapsed;
                    }
                });
                try
                {
                    bool isPlaying = MusicAPI.playbackInfo.PlaybackStatus == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Paused ;
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        SongTitle.Visibility = (Visibility)Convert.ToSByte(isPlaying);
                    });
                }
                catch
                {
                }
            }
            catch
            {

            }
            if (!CleanUp)
            {
                Thread.Sleep(1000);
                Thread thread = new(UpdateUI);
                thread.Start();
            }
        }
    }
}
