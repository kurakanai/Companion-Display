using CompanionDisplayWinUI.API;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.Web.WebView2.Core;
using Windows.System;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CompanionDisplayWinUI
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class WidgetBrowserView : Page
	{
        public Frame frame;
        public WidgetBrowserView()
        {
			this.InitializeComponent();
		}
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            WebView.GoBack();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            WebView.GoForward();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            WebView.Source = Globals.SearchEngine;
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            BrowserAPI.NavigateSpecialUrl(WebView, "downloads/all");
        }
        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            BrowserAPI.NavigateSpecialUrl(WebView, "history/all");
        }
        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            WebView.Reload();
        }

        private void AddressBar_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                WebView.Source = BrowserAPI.ParseLink(AddressBar.Text);
                WebView.Focus(FocusState.Programmatic);
            }
        }
        private void WebView_CoreWebView2Initialized(WebView2 sender, CoreWebView2InitializedEventArgs args)
        {
            WebView.CoreWebView2.NewWindowRequested += CoreWebView2_NewWindowRequested;
        }
        private void CoreWebView2_NewWindowRequested(object sender, CoreWebView2NewWindowRequestedEventArgs e)
        {
            e.NewWindow = WebView.CoreWebView2;
        }
        private void WebView_NavigationCompleted(WebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs args)
        {
            try
            {
                AddressBar.Text = WebView.Source.ToString();
            }
            catch { }
        }
        private async void WebView_Loaded(object sender, RoutedEventArgs e)
        {
            await BrowserAPI.CreateWebviewProperly(WebView, Globals.SearchEngine);
        }
    }
}
