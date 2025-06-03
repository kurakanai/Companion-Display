using CompanionDisplayWinUI.ClassImplementations;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Web.WebView2.Core;
using System;
using Windows.System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BlankPage2 : Page
    {
        public Frame frame;
        public BlankPage2(Uri link, NavigationView controlFS)
        {
            linkLoad = link;
            nv = controlFS;
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }
        private readonly NavigationView nv;
        public Uri linkLoad;
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
            BrowserClass.NavigateSpecialUrl(WebView, "downloads/all");
        }
        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            BrowserClass.NavigateSpecialUrl(WebView, "history/all");
        }
        private void AddressBar_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                WebView.Source = BrowserClass.ParseLink(AddressBar.Text);
                WebView.Focus(FocusState.Programmatic);
            }
        }
        private bool fullScreen = false;
        public bool FullScreen
        {
            get { return fullScreen; }
            set
            {
                fullScreen = value;
                if (fullScreen)
                {
                    MainGrid.Margin = new Thickness(-1);
                    WebView.Margin = new Thickness(0, -80, 0, 0);
                }
                else
                {
                    MainGrid.Margin = new Thickness(0);
                    WebView.Margin = MainGrid.Margin;
                }
                nv.IsPaneVisible = fullScreen;
                nv.IsPaneOpen = fullScreen;
            }
        }
        private void WebView_NavigationCompleted(WebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs args)
        {
            try
            {
                frame = this.Parent as Frame;
                (this.Parent as TabViewItem).Header = WebView.CoreWebView2.DocumentTitle.ToString();
                (this.Parent as TabViewItem).IconSource = new BitmapIconSource
                {
                    UriSource = new Uri(WebView.CoreWebView2.FaviconUri),
                    ShowAsMonochrome = false,
                };
            }
            catch { }
            WebView.CoreWebView2.ContainsFullScreenElementChanged += (obj, args) =>
            {
                this.FullScreen = WebView.CoreWebView2.ContainsFullScreenElement;
            };
            try
            {
                AddressBar.Text = WebView.Source.ToString();
                if (AddressBar.FocusState == FocusState.Pointer)
                {
                    AddressBar.SelectAll();
                }
            }
            catch { }
        }
        public void CloseTab()
        {
            WebView.Close();
        }
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                frame.Margin = new Thickness(0, 45, 0, 0);
            }
            catch
            {

            }
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            AddressBar.Focus(FocusState.Programmatic);
        }
        private bool FTU = true;
        private void Page_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                try
                {
                    WebView.Close();
                }
                catch { }
            });
        }

        private void AddressBar_GotFocus(object sender, RoutedEventArgs e)
        {
            AddressBar.SelectAll();
        }

        private async void WebView_Loaded(object sender, RoutedEventArgs e)
        {
            if (FTU)
            {
                Uri uri = null;
                if (linkLoad != null)
                {
                    uri = linkLoad;
                }
                else
                {
                    if (Globals.NewTabBehavior != 1)
                    {
                        uri = Globals.SearchEngine;
                    }
                }
                await BrowserClass.CreateWebviewProperly(WebView, uri);
                FTU = false;
            }
        }
        private void CoreWebView2_NewWindowRequested(object sender, CoreWebView2NewWindowRequestedEventArgs e)
        {
            e.NewWindow = WebView.CoreWebView2;
        }
        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            WebView.Reload();
        }

        private void WebView_CoreWebView2Initialized(WebView2 sender, CoreWebView2InitializedEventArgs args)
        {
            WebView.CoreWebView2.NewWindowRequested += CoreWebView2_NewWindowRequested;
        }
    }
}
