using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.Web.WebView2.Core;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace CompanionDisplayWinUI.API
{
    class BrowserAPI
    {
        private static readonly SemaphoreSlim semaphore = new(1);
        public static Uri ParseLink(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return BuildSearchUri(string.Empty);
            }
            input = input.Trim();
            if (Uri.TryCreate(input, UriKind.Absolute, out var directUri))
            {
                return directUri;
            }
            if (!input.Contains(' ') && Uri.TryCreate($"https://{input}", UriKind.Absolute, out var httpsUri) && httpsUri.Host.Contains('.'))
            {
                return httpsUri;
            }
            return BuildSearchUri(input);
        }
        private static Uri BuildSearchUri(string query)
            => new($"{Globals.SearchEngine}search?q={HttpUtility.UrlEncode(query)}");
        public static void NavigateSpecialUrl(WebView2 webView2, string url)
        {
            var targetUri = new Uri($"edge://{url}");

            if (webView2.Source == targetUri)
            {
                if (webView2.CanGoBack)
                {
                    webView2.GoBack();
                }
            }
            else
            {
                webView2.Source = targetUri;
            }
        }
        public static Button CreateLaunchPadButton(object content, FontFamily font, string name)
        {
            return new Button
            {
                Name = name,
                Height = 200,
                Width = 200,
                CornerRadius = new Microsoft.UI.Xaml.CornerRadius(8),
                FontFamily = font,
                Content = content,
                FontSize = 72,
                AllowDrop = true
            };
        }
        public static Image GetWebsiteIcon(Uri uri)
        {
            BitmapImage bitmapImage = new()
            {
                UriSource = new Uri($"https://www.google.com/s2/favicons?domain={uri.Host}&sz=256")
            };
            Image image = new()
            {
                Source = bitmapImage
            };
            return image;
        }
        public static CoreWebView2Environment sharedEnvironment;
        public async static Task CreateWebviewProperly(WebView2 webView2, Uri uri)
        {
            await semaphore.WaitAsync();
            try
            {
                if (sharedEnvironment == null)
                {
                    sharedEnvironment = await CoreWebView2Environment.CreateWithOptionsAsync(string.Empty, string.Empty, new() { AreBrowserExtensionsEnabled = true });
                }
                await webView2.EnsureCoreWebView2Async(sharedEnvironment);
                await webView2.CoreWebView2.Profile.AddBrowserExtensionAsync(Path.GetFullPath("Assets\\1.59.0_0"));
                webView2.Source = uri;
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}
