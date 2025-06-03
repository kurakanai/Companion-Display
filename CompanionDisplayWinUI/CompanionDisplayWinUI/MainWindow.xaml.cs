using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using WinRT.Interop;
using Microsoft.UI.Windowing;
using Microsoft.UI;
using Windows.UI;
using CompanionDisplayWinUI.ClassImplementations;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        [LibraryImport("user32.dll", EntryPoint = "GetWindowLongA")]
        private static partial int GetWindowLong(IntPtr hWnd, int nIndex);

        [LibraryImport("user32.dll", EntryPoint = "SetWindowLongA")]
        private static partial void SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        public MainWindow()
        {
            this.InitializeComponent();
            CommonlyAccessedInstances.nvSample = nvSample;
            this.ExtendsContentIntoTitleBar = true;
            SetTitleBar(AppTitleBar);
            _appWindow = GetAppWindowForCurrentWindow();
            Thread thread = new(UpdateUI);
            thread.Start();
        }
        private AppWindow GetAppWindowForCurrentWindow()
        {
            IntPtr hWnd = WindowNative.GetWindowHandle(this);
            WindowId myWndId = Win32Interop.GetWindowIdFromWindow(hWnd);
            return AppWindow.GetFromWindowId(myWndId);
        }
        private void NavigationView_SelectionChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            if (sender.IsPaneToggleButtonVisible && sender.PaneDisplayMode == NavigationViewPaneDisplayMode.Auto || sender.PaneDisplayMode == NavigationViewPaneDisplayMode.LeftMinimal && !sender.IsPaneToggleButtonVisible)
            {
                try
                {
                    GC.Collect();
                    Globals.IsAllApps = false;
                    IntPtr hWnd = WindowNative.GetWindowHandle(this);
                    sender.PaneClosed -= ProcessShit;
                    switch (args.SelectedItemContainer.Tag.ToString())
                    {
                        case "SecretStuff":
                            contentFrame.Navigate(typeof(MusixmatchIntegrationProto), null, args.RecommendedNavigationTransitionInfo);
                            SetWindowLong(hWnd, -20, 256);
                            sender.PaneClosed += ProcessShit;
                            break;
                        case "SamplePage1":
                            SleepReptangle.Visibility = Visibility.Collapsed;
                            contentFrame.Navigate(typeof(BlankPage1), null, args.RecommendedNavigationTransitionInfo);
                            if (Globals.StealFocus)
                            {
                                SetWindowLong(hWnd, -20, GetWindowLong(hWnd, -20) | 134480128);
                            }
                            sender.PaneClosed += ProcessShit;
                            break;
                        case "SamplePage2":
                            SleepReptangle.Visibility = Visibility.Collapsed;
                            contentFrame.Navigate(typeof(BrowserTab), null, args.RecommendedNavigationTransitionInfo);
                            SetWindowLong(hWnd, -20, 256);
                            sender.PaneClosed += ProcessShit;
                            break;
                        case "SamplePage3":
                            SleepReptangle.Visibility = Visibility.Collapsed;
                            contentFrame.Navigate(typeof(SpotifyPlayer), null, args.RecommendedNavigationTransitionInfo);
                            SetWindowLong(hWnd, -20, 256);
                            sender.PaneClosed += ProcessShit;
                            break;
                        case "SleepMode":
                            SleepReptangle.Visibility = Visibility.Visible;
                            contentFrame.Navigate(typeof(SleepPage), null, args.RecommendedNavigationTransitionInfo);
                            if (Globals.StealFocus)
                            {
                                SetWindowLong(hWnd, -20, GetWindowLong(hWnd, -20) | 134480128);
                            }
                            break;
                        case "Settings":
                            SleepReptangle.Visibility = Visibility.Collapsed;
                            SetWindowLong(hWnd, -20, 256);
                            contentFrame.Navigate(typeof(BlankPage3), null, args.RecommendedNavigationTransitionInfo);
                            sender.PaneClosed += ProcessShit;
                            break;
                    }
                }
                catch { }
            }
        }

        private void ProcessShit(NavigationView sender, object args)
        {
            if (!sender.IsPaneToggleButtonVisible)
            {
                sender.PaneDisplayMode = NavigationViewPaneDisplayMode.Auto;
                sender.IsPaneToggleButtonVisible = !sender.IsPaneToggleButtonVisible;
                sender.PaneClosed -= ProcessShit;
            }
        }

        private void Window_Closed(object sender, WindowEventArgs args)
        {
            Globals.StartedPlayer = false;
            Environment.Exit(0);
        }
        private void SongCoverBackground()
        {
            Media.GetCover(DispatcherQueue, BackgroundImage);
        }
        private void UpdateUI()
        {
            try
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    BackgroundImage.Visibility = Visibility.Collapsed;
                    ImageOptionalBlur.Visibility = Visibility.Collapsed;
                    SystemBackdrop = null;
                    GridMain.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                });
                switch (Globals.Backdrop)
                {
                    case (0):
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            SystemBackdrop = new DesktopAcrylicBackdrop();
                        });
                        break;
                    case (1):
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            SystemBackdrop = new MicaBackdrop();
                        });
                        break;
                    case (2):
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            ImageOptionalBlur.Visibility = (Visibility)Convert.ToByte(!Globals.Blur);
                        });
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            try
                            {
                                if (Globals.BackgroundLink == "")
                                {
                                    string imagePath = Globals.Wallpaper;
                                    BitmapImage bitmapImage = new();
                                    using (var stream = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                                    {
                                        bitmapImage.SetSource(stream.AsRandomAccessStream());
                                    }
                                    BackgroundImage.Source = bitmapImage;
                                }
                                else
                                {
                                    BitmapImage bitmapImage = new()
                                    {
                                        UriSource = new Uri(Globals.BackgroundLink)
                                    };
                                    BackgroundImage.Source = bitmapImage;
                                }
                                BackgroundImage.Visibility = Visibility.Visible;
                            }
                            catch { }
                        });
                        break;
                    case (3):
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            BackgroundImage.Visibility = Visibility.Visible;
                            if (Globals.Blur == true)
                            {
                                ImageOptionalBlur.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                ImageOptionalBlur.Visibility = Visibility.Collapsed;
                            }
                        });
                        Thread thread2 = new(SongCoverBackground);
                        thread2.Start();
                        Media.CallInfoUpdate += SongCoverBackground;
                        break;
                    case (4):
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            GridMain.Background = new SolidColorBrush(Color.FromArgb(255, (byte)Globals.BackgroundColorR, (byte)Globals.BackgroundColorG, (byte)Globals.BackgroundColorB));
                        });
                        break;
                }
            }
            catch
            {

            }
        }
        public void CallUpdate()
        {
            Thread thread = new(UpdateUI);
            thread.Start();
        }
        private readonly AppWindow _appWindow;

        private void BackgroundImage_ImageOpened(object sender, RoutedEventArgs e)
        {
            if (ImageOptionalBlur.Visibility == Visibility.Visible)
            {
                ImageOptionalBlur.Stretch = Stretch.None;
                ImageOptionalBlur.Stretch = Stretch.UniformToFill;
            }
            try
            {
                if (!Globals.useLessDemandingEffects)
                {
                    (contentFrame.Content as BlankPage1).ForceBugcheck();
                }
            }
            catch { }
        }

        private void DebugPage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            bool fullscreen = _appWindow.Presenter.Kind == AppWindowPresenterKind.FullScreen;
            if (fullscreen)
            {
                CornerMask.Visibility = Visibility.Visible;
                AppTitleBar.Visibility = Visibility.Visible;
                _appWindow.SetPresenter(AppWindowPresenterKind.Default);
                DebugPage.Content = AppStrings.fullscrenEnter;
                DebugPage.Icon = new SymbolIcon(Symbol.FullScreen);
            }
            else
            {
                CornerMask.Visibility = Visibility.Collapsed;
                AppTitleBar.Visibility = Visibility.Collapsed;
                _appWindow.SetPresenter(AppWindowPresenterKind.FullScreen);
                DebugPage.Content = AppStrings.fullscrenExit;
                DebugPage.Icon = new SymbolIcon(Symbol.BackToWindow);
            }
            this.ExtendsContentIntoTitleBar = fullscreen;
        }

        private void NvSample_Loaded(object sender, RoutedEventArgs e)
        {
            if (Globals.triggerSetup)
            {
                (sender as NavigationView).IsPaneVisible = !Globals.triggerSetup;
                contentFrame.Navigate(typeof(SetupStep0));
            }
        }
    }
}
