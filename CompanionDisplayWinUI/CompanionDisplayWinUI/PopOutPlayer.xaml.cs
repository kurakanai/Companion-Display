using CompanionDisplayWinUI.API;
using Microsoft.UI.Xaml;
using Microsoft.Web.WebView2.Core;
using System;
using System.IO;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PopOutPlayer : Window
    {
        public PopOutPlayer(Uri popoutLink)
        {
            this.InitializeComponent();
            this.ExtendsContentIntoTitleBar = true;
            link = popoutLink;
        }
        public Uri link;
        private bool FTU = true;
        private async void Player_Loaded(object sender, RoutedEventArgs e)
        {
            if (FTU)
            {
                await BrowserAPI.CreateWebviewProperly(Player, link);
                FTU = false;
            }
            Player.Source = link;
        }
    }
}
