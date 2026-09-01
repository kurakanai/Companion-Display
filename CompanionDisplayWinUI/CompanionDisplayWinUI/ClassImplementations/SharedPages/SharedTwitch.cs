using CompanionDisplayWinUI.API;
using CompanionDisplayWinUI.Objects;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace CompanionDisplayWinUI.ClassImplementations.SharedPages
{
    public class SharedTwitch : CommonWidget
    {
        public WebView2 pageWebView;
        public string destinationUrl;
        public bool FTU = true;
        public async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (FTU)
            {
                await BrowserAPI.CreateWebviewProperly(pageWebView, new Uri(destinationUrl));
                FTU = !FTU;
            }
        }
        public void Button_Click(object sender, RoutedEventArgs e)
        {
            PopOutPlayer m_window = new(pageWebView.Source);
            m_window.Activate();
        }
        public void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            pageWebView.CoreWebView2.Reload();
        }
    }
}
