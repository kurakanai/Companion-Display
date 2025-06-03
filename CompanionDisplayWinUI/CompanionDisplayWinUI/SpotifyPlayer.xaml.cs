using CompanionDisplayWinUI.ClassImplementations;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SpotifyPlayer : Page
    {
        public SpotifyPlayer()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }
        private bool FTU = true;
        private async void SpotifyBuiltin_Loaded(object sender, RoutedEventArgs e)
        {
            if (FTU)
            {
                await BrowserClass.CreateWebviewProperly(SpotifyBuiltin, new Uri("https://open.spotify.com/"));
                FTU = !FTU;
            }
        }
    }
}
