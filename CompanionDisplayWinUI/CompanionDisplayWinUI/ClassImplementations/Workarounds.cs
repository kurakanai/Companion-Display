using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace CompanionDisplayWinUI.ClassImplementations
{
    public static class Workarounds
    {
        public static void BugcheckAcrylic(object sender, RoutedEventArgs _1)
        {
            if (Globals.Backdrop <= 1 || Globals.useLessDemandingEffects)
            {
                var uiSettings = new Windows.UI.ViewManagement.UISettings();
                (sender as Frame).Background = null;
                (sender as Frame).Background = new SolidColorBrush(uiSettings.GetColorValue(Windows.UI.ViewManagement.UIColorType.Background))
                {
                    Opacity = 0.3
                };
            }
            else
            {
                (sender as Frame).Background = (AcrylicBrush)Application.Current.Resources["CustomAcrylicInAppLuminosity"];
                (sender as Frame).Background.Opacity = 1;
            }
        }
        public static void ForceBugcheckFrames(GridView gridView)
        {
            foreach (var children in gridView.Items)
            {
                if (children as Frame != null)
                {
                    (children as Frame).Background = null;
                    (children as Frame).Background = (AcrylicBrush)Application.Current.Resources["CustomAcrylicInAppLuminosity"];
                }
            }
        }
    }
}
