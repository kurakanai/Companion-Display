using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using System;
using Windows.UI;
using System.Threading;
using Windows.System;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Drawing.Text;
using Microsoft.UI.Xaml.Media.Animation;
using CompanionDisplayWinUI.ClassImplementations;
using System.Threading.Tasks;
using System.IO;

namespace CompanionDisplayWinUI
{
    public sealed partial class BlankPage3 : Page
    {
        public bool LoadFinish = false;
        public BlankPage3()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
            _ = LoadValues();
        }
        private async Task LoadValues()
        {
            if (System.IO.File.Exists("Config/SE.crlh"))
            {
                AppIconImg.Source = new BitmapImage(new Uri("https://i.imgur.com/ng8AhkJ.jpeg"));
            }
            if (Globals.IgnoreUpdates)
            {
                await UpdateSystem.CheckUpdate();
            }
            if (Globals.IsUpdateAvailable)
            {
                UpdateBtn.Content = AppStrings.updateUpdate;
            }
            else
            {
                UpdateBtn.Content = AppStrings.updateUpToDate;
            }
            UpdateBtn.IsEnabled = Globals.IsUpdateAvailable;
            ColorSchemeSelect.SelectedIndex = Globals.ColorSchemeSelect;
            AccentSelect.SelectedIndex = Globals.InjectCustomAccent;
            AccentColorPicker.Color = Color.FromArgb(255, (byte)Globals.ColorSchemeSelectAccentR, (byte)Globals.ColorSchemeSelectAccentG, (byte)Globals.ColorSchemeSelectAccentB);
            BackdropSelect.SelectedIndex = Globals.Backdrop;
            BackgroundLink.Text = Globals.BackgroundLink;
            ImageBlurToggle.IsOn = Globals.Blur;
            VersionString.Text = Globals.Version;
            FocusToggle.IsOn = Globals.StealFocus;
            UpdateToggle.IsOn = Globals.IsBetaProgram;
            AddButtonToggle.IsOn = Globals.HideAddButton;
            StartupToggle.IsOn = Globals.LaunchOnStartup;
            LockToggle.IsOn = Globals.LockLayout;
            OpacitySlider.Value = Globals.sleepModeOpacity;
            OvrColorSleepMode.IsOn = Globals.OverrideColor;
            SleepModeColor.Color = Color.FromArgb(255, (byte)Globals.SleepColorR, (byte)Globals.SleepColorG, (byte)Globals.SleepColorB);
            SearchEngineCust.Text = Globals.SearchEngine.ToString();
            NewTabBehavior.SelectedIndex = Globals.NewTabBehavior;
            OBSIP.Text = Globals.obsIP;
            OBSPass.Password = Globals.obsPass;
            if (Globals.obsControls.connectionSuccessful)
            {
                OBSStatus.Text = AppStrings.obsConnected;
            }
            else
            {
                OBSStatus.Text = AppStrings.obsDisconnected;
            }
            InstalledFontCollection fontCollection = new();
            foreach (var fontFamily in fontCollection.Families)
            {
                MenuFlyoutItem item = new()
                {
                    Text = fontFamily.Name,
                    FontFamily = new Microsoft.UI.Xaml.Media.FontFamily(fontFamily.Name)
                };
                item.Click += MenuFlyoutItem_Click;
                FontSelectorActually.Items.Add(item);
            }
            FontSelector.Content = ThemingAndColors.CurrentFont();
            SoundsToggle.IsOn = Globals.enableUISounds;
            UseLessIntensiveUI.IsOn = Globals.useLessDemandingEffects;
            TwelveHourToggle.IsOn = Globals.use12HourClock;
            PromoToggle.IsOn = Globals.showPromo;
            UpdateNagToggle.IsOn = Globals.IgnoreUpdates;
            DiscordToggle.IsOn = Globals.disableDiscord;
            LoadFinish = true;
        }
        private void ProcessShit(NavigationView sender, object args)
        {
            sender.PaneDisplayMode = NavigationViewPaneDisplayMode.Auto;
            sender.PaneClosed -= ProcessShit;
        }
        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            ThemingAndColors.SetFont(new Microsoft.UI.Xaml.Media.FontFamily((sender as MenuFlyoutItem).Text));
            FontSelector.Content = (sender as MenuFlyoutItem).Text;
            ConfigurationOperations.Save_Settings();
            (mainframe.Parent as NavigationView).PaneClosed += ProcessShit;
            (mainframe.Parent as NavigationView).PaneDisplayMode = NavigationViewPaneDisplayMode.LeftMinimal;
            mainframe.Navigate(typeof(BlankPage1));
            mainframe.Navigate(typeof(BlankPage3));
        }
        private Frame mainframe;
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LoadFinish)
            {
                int selection = (sender as ComboBox).SelectedIndex;
                switch (selection)
                {
                    case 0:
                        ThemingAndColors.SetAppTheme(ElementTheme.Default);
                        break;
                    case 1:
                        ThemingAndColors.SetAppTheme(ElementTheme.Dark);
                        break;
                    case 2:
                        ThemingAndColors.SetAppTheme(ElementTheme.Light);
                        break;
                }
                Globals.ColorSchemeSelect = ColorSchemeSelect.SelectedIndex;
                (CommonlyAccessedInstances.m_window as MainWindow).CallUpdate();
                ConfigurationOperations.Save_Settings();
            }
        }
        private void ComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (LoadFinish)
            {
                int selection = (sender as ComboBox).SelectedIndex;
                var Parent = MainScroll.Parent as BlankPage3;
                switch (selection)
                {
                    case 0:
                        ThemingAndColors.RevertToSystemAccentColor();
                        break;
                    case 1:
                        ThemingAndColors.SetAccentColor(Color.FromArgb(255, (byte)Globals.ColorSchemeSelectAccentR, (byte)Globals.ColorSchemeSelectAccentG, (byte)Globals.ColorSchemeSelectAccentB));
                        break;
                }
                Globals.InjectCustomAccent = AccentSelect.SelectedIndex;
                ConfigurationOperations.Save_Settings();
                mainframe.Navigate(typeof(BlankPage1));
                mainframe.Navigate(typeof(BlankPage3));
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            mainframe = this.Parent as Frame;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (LoadFinish)
            {
                Globals.ColorSchemeSelectAccentR = AccentColorPicker.Color.R;
                Globals.ColorSchemeSelectAccentG = AccentColorPicker.Color.G;
                Globals.ColorSchemeSelectAccentB = AccentColorPicker.Color.B;
                if (Globals.InjectCustomAccent == 1)
                {
                    ThemingAndColors.SetAccentColor(Color.FromArgb(255, (byte)Globals.ColorSchemeSelectAccentR, (byte)Globals.ColorSchemeSelectAccentG, (byte)Globals.ColorSchemeSelectAccentB));
                }
                ConfigurationOperations.Save_Settings();
            }
        }

        private void ComboBox_SelectionChanged_2(object sender, SelectionChangedEventArgs e)
        {
            if(LoadFinish)
            {
                string Theme = e.AddedItems[0].ToString();
                Globals.Backdrop = BackdropSelect.SelectedIndex;
                (CommonlyAccessedInstances.m_window as MainWindow).CallUpdate();
                ConfigurationOperations.Save_Settings();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (LoadFinish)
            {
                string btntag = FileFolderPicker.OpenFileDialog(false)[0];
                if (btntag != null)
                {
                    Globals.Wallpaper = btntag;
                }
                ConfigurationOperations.Save_Settings();
                (CommonlyAccessedInstances.m_window as MainWindow).CallUpdate();
            }
        }

        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (LoadFinish)
            {
                Globals.StealFocus = FocusToggle.IsOn;
                ConfigurationOperations.Save_Settings();
            }
        }

        private void ImageBlurToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (LoadFinish)
            {
                Globals.Blur = ImageBlurToggle.IsOn;
                ConfigurationOperations.Save_Settings();
                (CommonlyAccessedInstances.m_window as MainWindow).CallUpdate();
            }
        }

        private void BackgroundLink_LostFocus(object sender, RoutedEventArgs e)
        {
            Globals.BackgroundLink = BackgroundLink.Text;
            ConfigurationOperations.Save_Settings();
            (CommonlyAccessedInstances.m_window as MainWindow).CallUpdate();
        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri(AppStrings.devGithubUrl));
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            UpdateBtn.IsEnabled = false;
            UpdateLocalBtn.IsEnabled = false;
            UpdateBtn.Content = AppStrings.updateUpdating;
            UpdateSystem.PerformUpdate(true);
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            if (LoadFinish)
            {
                Globals.BackgroundColorR = BackgroundColorPicker.Color.R;
                Globals.BackgroundColorG = BackgroundColorPicker.Color.G;
                Globals.BackgroundColorB = BackgroundColorPicker.Color.B;
                ConfigurationOperations.Save_Settings();
                (CommonlyAccessedInstances.m_window as MainWindow).CallUpdate();
            }
        }

        private void UpdateToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (LoadFinish)
            {
                Globals.IsBetaProgram = UpdateToggle.IsOn;
                ConfigurationOperations.Save_Settings();
            }
        }

        private void AddButtonToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (LoadFinish)
            {
                Globals.HideAddButton = AddButtonToggle.IsOn;
                ConfigurationOperations.Save_Settings();
            }
        }

        private void StartupToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (LoadFinish)
            {
                if (StartupToggle.IsOn)
                {
                    try
                    {
                        ShortcutWorkings.CreateShortcut(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), "Companion Display.lnk"), AppStrings.appShortcutDescription, System.IO.Path.Combine(System.IO.Path.GetFullPath(Environment.ProcessPath.ToString())), System.IO.Path.Combine(System.IO.Path.GetFullPath(Environment.CurrentDirectory.ToString())));
                    }
                    catch
                    {
                        AddButtonToggle.IsOn = false;
                    }
                }
                else
                {
                    FileOperations.DeleteFile(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), "Companion Display.lnk"));
                }
                Globals.LaunchOnStartup = StartupToggle.IsOn;
                ConfigurationOperations.Save_Settings();
            }
        }

        private void LockToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (LoadFinish)
            {
                Globals.LockLayout = LockToggle.IsOn;
                ConfigurationOperations.Save_Settings();
            }
        }

        private async void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri(AppStrings.devPaypalUrl));
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            if (LoadFinish)
            {
                Globals.SleepColorR = SleepModeColor.Color.R;
                Globals.SleepColorG = SleepModeColor.Color.G;
                Globals.SleepColorB = SleepModeColor.Color.B;
                ConfigurationOperations.Save_Settings();
            }
        }

        private void Opacity_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            Globals.sleepModeOpacity = (sender as Slider).Value;
            ConfigurationOperations.Save_Settings();
        }

        private void SearchEngineCust_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if((sender as TextBox).Text != "")
                {
                    Globals.SearchEngine = new Uri("https://" + (sender as TextBox).Text.Replace("https://", "").Replace("http://", ""));
                }
                else
                {
                    Globals.SearchEngine = new Uri("https://www.google.com/");
                }
                ConfigurationOperations.Save_Settings();
            }
            catch
            {

            }
        }

        private void NewTabBehavior_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Globals.NewTabBehavior = (sender as ComboBox).SelectedIndex;
            ConfigurationOperations.Save_Settings();
        }

        private async void ResetTwitch_Click(object sender, RoutedEventArgs e)
        {
            await BrowserClass.CreateWebviewProperly(resetTwitch, new Uri("about:blank"));
            resetTwitch.CoreWebView2Initialized += DeleteCookies;
        }

        private async void DeleteCookies(WebView2 sender, CoreWebView2InitializedEventArgs args)
        {
            System.Collections.Generic.IReadOnlyList<Microsoft.Web.WebView2.Core.CoreWebView2Cookie> cookies = await resetTwitch.CoreWebView2.CookieManager.GetCookiesAsync("https://twitch.tv");
            foreach(var Cookie in cookies)
            {
                resetTwitch.CoreWebView2.Profile.CookieManager.DeleteCookie(Cookie);
            }
            resetTwitch.Close();
        }

        private void Opacity_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Globals.sleepModeOpacity = (sender as Slider).Value;
            ConfigurationOperations.Save_Settings();
        }

        private void OBSIP_TextChanged(object sender, TextChangedEventArgs e)
        {
            if((sender as TextBox).Text == "")
            {
                Globals.obsIP = (sender as TextBox).PlaceholderText;
            }
            else
            {
                Globals.obsIP = (sender as TextBox).Text;
            }
            ConfigurationOperations.SaveOBSConfig();
        }

        private void ReconnectOBS_Click(object sender, RoutedEventArgs e)
        {
            Thread thread = new(Globals.obsControls.ManualConnectReq);
            thread.Start();
            System.Timers.Timer timer = new(3000) { Enabled = true };
            (sender as Button).IsEnabled = false;
            timer.Elapsed += (sender, args) =>
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    if (!Globals.obsControls.connectionSuccessful)
                    {
                        OBSStatus.Text = AppStrings.obsDisconnected;
                        ReconnectOBS.Content = AppStrings.obsConnectionFailed;
                        timer.Dispose();
                    }
                    else
                    {
                        OBSStatus.Text = AppStrings.obsConnected;
                        ReconnectOBS.Content = AppStrings.obsReconnect;
                    }
                    ReconnectOBS.IsEnabled = true;
                });
            };
        }

        private void OBSPass_PasswordChanged(object sender, RoutedEventArgs e)
        {
            Globals.obsPass = (sender as PasswordBox).Password;
            Thread thread = new(ConfigurationOperations.SaveOBSConfig);
            thread.Start();
        }

        private void SetupBtn_Click(object sender, RoutedEventArgs e)
        {
            var frame = this.Parent as Frame;
            CommonlyAccessedInstances.nvSample.IsPaneVisible = false;
            frame.Navigate(typeof(SetupStep0), null, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
        }

        private void BackupBtn_Click(object sender, RoutedEventArgs e)
        {
            BackupOperations.BackupFinished += HidePane;
            BackupGrid.Visibility = Visibility.Visible;
            BackupOperations.OpenDialog(this.XamlRoot, true);
        }

        private void HidePane()
        {
            BackupOperations.BackupFinished -= HidePane;
            BackupGrid.Visibility = Visibility.Collapsed;
        }

        private void OvrColorSleepMode_Toggled(object sender, RoutedEventArgs e)
        {
            Globals.OverrideColor = OvrColorSleepMode.IsOn;
            ConfigurationOperations.Save_Settings();
        }

        private void SoundsToggle_Toggled(object sender, RoutedEventArgs e)
        {
            Globals.enableUISounds = SoundsToggle.IsOn;
            ElementSoundPlayer.State = (ElementSoundPlayerState)(Convert.ToByte(Globals.enableUISounds) + 1);
            ConfigurationOperations.Save_Settings_Background();
        }

        private void UseLessIntensiveUI_Toggled(object sender, RoutedEventArgs e)
        {
            Globals.useLessDemandingEffects = UseLessIntensiveUI.IsOn;
            ThemingAndColors.ImageOptionalBlur_Loaded();
            ConfigurationOperations.Save_Settings();
        }

        private void TwelveHourToggle_Toggled(object sender, RoutedEventArgs e)
        {
            Globals.use12HourClock = TwelveHourToggle.IsOn;
            ConfigurationOperations.Save_Settings();
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            UpdateBtn.IsEnabled = false;
            UpdateLocalBtn.IsEnabled = false;
            UpdateSystem.PerformUpdate(false);
            UpdateBtn.IsEnabled = Globals.IsUpdateAvailable;
            UpdateLocalBtn.IsEnabled = true;
        }

        private void PromoToggle_Toggled(object sender, RoutedEventArgs e)
        {
            Globals.showPromo = PromoToggle.IsOn;
            ConfigurationOperations.Save_Settings_Background();
        }

        private void UpdateNagToggle_Toggled(object sender, RoutedEventArgs e)
        {
            Globals.IgnoreUpdates = UpdateNagToggle.IsOn;
            ConfigurationOperations.Save_Settings_Background();
        }

        private void ResetBrowser_Click(object sender, RoutedEventArgs e)
        {
            CMDOperations.PerformCMDCommand("taskkill /im msedgewebview2.exe");
            FileOps.deleteDirectoryRecursive("CompanionDisplayWinUI.exe.WebView2");
        }

        private void DiscordToggle_Toggled(object sender, RoutedEventArgs e)
        {
            Globals.disableDiscord = DiscordToggle.IsOn;
            if (Globals.disableDiscord)
            {
               Globals.playerSpotify.client.Deinitialize();
            }
            else
            {
                Globals.playerSpotify.LoadDiscordRPC();
            }
            ConfigurationOperations.Save_Settings();
        }
    }
}
