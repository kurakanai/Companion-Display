using System;
using System.IO;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Animation;
using Windows.System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SetupStep1 : Page
    {
        public SetupStep1()
        {
            this.InitializeComponent();
        }

        private void Button_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var frame = this.Parent as Frame;
            frame.GoBack();
        }

        private void Button_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            Globals._clientId = SPCli_Field.Text;
            Globals._secretId = SPCliSecret_Field.Text;
            Globals._clientId2 = SPCli2_Field.Text;
            Globals._secretId2 = SPCliSecret2_Field.Text;
            var frame = this.Parent as Frame;
            frame.Navigate(typeof(SetupStep2), null, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (File.Exists(Globals.MediaConfigFile))
            {
                string config = File.ReadAllText(Globals.MediaConfigFile);
                using StringReader readerconfig = new(config);
                {
                    Globals._clientId = readerconfig.ReadLine();
                    Globals._secretId = readerconfig.ReadLine();
                    Globals._clientId2 = readerconfig.ReadLine();
                    Globals._secretId2 = readerconfig.ReadLine();
                }
            }
            if (File.Exists(Globals.RefreshTokenPath))
            {
                File.Delete(Globals.RefreshTokenPath);
            }
            if (File.Exists(Globals.RefreshToken2Path))
            {
                File.Delete(Globals.RefreshToken2Path);
            }
            SPCli_Field.Text = Globals._clientId;
            SPCliSecret_Field.Text = Globals._secretId;
            SPCli2_Field.Text = Globals._clientId2;
            SPCliSecret2_Field.Text = Globals._secretId2;
        }

        private void HyperlinkButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var frame = this.Parent as Frame;
            frame.Navigate(typeof(SetupStep2), null, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            string url = "https://github.com/kurakanai/Spotify-Lyrics-For-Discord/wiki/User-Guides#" + (sender as Button).Tag;
            await Launcher.LaunchUriAsync(new Uri(url));
        }
    }
}
