using CompanionDisplayWinUI.ClassImplementations;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TwitchActivityFeedWidget : Page
    {
        public TwitchActivityFeedWidget()
        {
            this.InitializeComponent();
        }

        private bool FTU = true;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PopOutPlayer m_window = new(Player.Source);
            m_window.Activate();
        }
        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            Player.CoreWebView2.Reload();
        }

        private async void Player_Loaded(object sender, RoutedEventArgs e)
        {
            if (FTU)
            {
                await BrowserClass.CreateWebviewProperly(Player, new Uri("https://dashboard.twitch.tv/popout/stream-manager/activity-feed"));
                FTU = !FTU;
            }
        }
    }
}
