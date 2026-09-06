using CompanionDisplayWinUI.API;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI.Pages.Widgets.Twitch.SharedAssets
{
    public sealed partial class BaseTwitchWrapper : UserControl
    {
        public BaseTwitchWrapper()
        {
            InitializeComponent();
            
        }
        public static readonly DependencyProperty LinkProperty =
        DependencyProperty.Register(
            nameof(Link),
            typeof(string),
            typeof(BaseTwitchWrapper),
            new PropertyMetadata(""));

        public string Link
        {
            get => (string)GetValue(LinkProperty);
            set {
                SetValue(LinkProperty, value);
                InitWidget(value);
            }
        }
        private async void InitWidget(string link){
            await BrowserAPI.CreateWebviewProperly(Player, new Uri(link), false);
        }
        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            if(Player.CoreWebView2 != null){
                Player.CoreWebView2.Reload();
            }
        }

        private void HyperlinkButton_Click_1(object sender, RoutedEventArgs e)
        {
            PopOutPlayer m_window = new(Player.Source);
            m_window.Activate();
        }
    }
}
