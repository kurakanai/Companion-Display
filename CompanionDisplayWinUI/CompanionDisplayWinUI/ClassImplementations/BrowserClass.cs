using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.Web.WebView2.Core;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace CompanionDisplayWinUI.ClassImplementations
{
    class BrowserClass
    {
        private static readonly SemaphoreSlim semaphore = new(1); // Limit to 2 concurrent initializations
        public static Uri ParseLink(string input)
        {
            if (input.Length > 3 && input[1..].Contains("://"))
            {
                return(new Uri(input));
            }
            else if (input.Length > 3 && input[1..].Contains('.') && input[..^2].Contains('.') && !input.Contains(' '))
            {
                try
                {
                    return(new Uri("https://" + input));
                }
                catch
                {
                    return(new Uri(Globals.SearchEngine + "/search?q=" + HttpUtility.UrlEncode(input)));
                }
            }
            else
            {
                return(new Uri(Globals.SearchEngine + "/search?q=" + HttpUtility.UrlEncode(input)));
            }
        }
        public static void NavigateSpecialUrl(WebView2 webView2, string url)
        {
            if (webView2.Source != new Uri("edge://" + url))
            {
                webView2.Source = new Uri("edge://" + url);
            }
            else
            {
                webView2.GoBack();
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
                UriSource = new System.Uri("https://www.google.com/s2/favicons?domain=" + uri.Host + "&sz=256")
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
                    sharedEnvironment = await CoreWebView2Environment.CreateWithOptionsAsync("", "", new() { AreBrowserExtensionsEnabled = true });
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
