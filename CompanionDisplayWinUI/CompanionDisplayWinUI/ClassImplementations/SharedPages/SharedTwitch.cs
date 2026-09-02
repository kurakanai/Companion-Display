using CompanionDisplayWinUI.API;
using CompanionDisplayWinUI.Objects;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace CompanionDisplayWinUI.ClassImplementations.SharedPages
{
    public partial class SharedTwitch : CommonWidget
    {
        public WebView2 pageWebView;
        public string destinationUrl;
        public bool FTU = true;
        public async void Page_Loaded(object _1, RoutedEventArgs _2)
        {
            if (FTU)
            {
                await BrowserAPI.CreateWebviewProperly(pageWebView, new Uri(destinationUrl));
                FTU = !FTU;
            }
        }
        public void Button_Click(object _1, RoutedEventArgs _2)
        {
            PopOutPlayer m_window = new(pageWebView.Source);
            m_window.Activate();
        }
        public void HyperlinkButton_Click(object _1, RoutedEventArgs _2)
        {
            pageWebView.CoreWebView2.Reload();
        }
    }
}
