using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using static CompanionDisplayWinUI.MediaPlayerWidget;

namespace CompanionDisplayWinUI.ClassImplementations
{
    static class Media
    {
        public static event CommonlyAccessedInstances.HandleEventsWithNoArgs CallInfoUpdate, CallTimingUpdate, CallLyricUpdate;
        public static int currentLyric = 0;
        public static double SongProgress;
        public static string SongName, SongDetails, SongLyrics, SongTime, SongEnd, SongBackground, AlbumName, ArtistName, SongBackgroundDiscord;
        public static string[] Lyrics;
        public static string nonTimedLyrics;
        public static double[] LyricTimings;
        public static void SongChanged()
        {
            CallInfoUpdate?.Invoke();
        }
        public static void SongTimingChanged()
        {
            CallTimingUpdate?.Invoke();
        }
        public static void SongLyricChanged()
        {
            CallLyricUpdate?.Invoke();
        }
        public static void SetLyrics(int index, string lyrics)
        {
            currentLyric = index;
            SongLyrics = lyrics;
            Media.SongLyricChanged();
        }
        public static void SetTime(string currentTime, string duration, double progress)
        {
            SongTime = currentTime;
            SongEnd = duration;
            SongProgress = progress;
            Media.SongTimingChanged();
        }
        public async static void OpenSleepDialogue(XamlRoot xamlRoot)
        {
            ContentDialog dialog = new()
            {
                XamlRoot = xamlRoot,
                Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                DefaultButton = ContentDialogButton.Primary
            };
            if (Globals.sleepTimer.isEnabled)
            {
                dialog.Title = AppStrings.sleepTimerEnd;
                dialog.Content = AppStrings.sleepTimerAlreadyActive;
                dialog.PrimaryButtonText = AppStrings.sleepTimerEnd;
                dialog.CloseButtonText = AppStrings.cancelString;
                var result = await dialog.ShowAsync();
                if (result == ContentDialogResult.Primary && Globals.sleepTimer.isEnabled)
                {
                    Globals.sleepTimer.CancelTimer();
                }
            }
            else
            {
                dialog.Title = AppStrings.sleepTimer;
                dialog.PrimaryButtonText = AppStrings.sleepTimerStart;
                dialog.CloseButtonText = AppStrings.cancelString;
                NumberBox numberBox = new()
                {
                    PlaceholderText = AppStrings.sleepTimerMinsPlaceholder,
                    SpinButtonPlacementMode = NumberBoxSpinButtonPlacementMode.Inline,
                    SmallChange = 1,
                    LargeChange = 5,
                    Minimum = 1
                };
                dialog.Content = numberBox;
                var result = await dialog.ShowAsync();
                if (result == ContentDialogResult.Primary && !Globals.sleepTimer.isEnabled)
                {
                    Globals.sleepTimer.StartTimer((int)numberBox.Value);
                }
            }
        }
        public static void GetCover(DispatcherQueue dispatcherQueue, Image image)
        {
            if (Globals.IsSpotify || (SongBackground != null && SongBackground.Length > 0))
            {
                dispatcherQueue.TryEnqueue(() =>
                {
                    try
                    {
                        image.Source = new BitmapImage(new Uri(SongBackground));
                    }
                    catch { }
                });
            }
            else
            {
                dispatcherQueue.TryEnqueue(() =>
                {
                    try
                    {
                        image.Source = Helper.GetThumbnail(Globals.songInfo.Thumbnail);
                    }
                    catch { }
                });
            }
        }
    }
}
