using CompanionDisplayWinUI.ClassImplementations;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using System;
using Windows.UI;
using Windows.UI.ViewManagement;

namespace CompanionDisplayWinUI.API
{
    static class ThemingAPI
    {
        private static readonly ResourceDictionary customResourceDictionary = Application.Current.Resources;
        private static readonly UISettings uiSettings = new();
        private static ResourceDictionary _customAccentDictionary;
        public static void SetAccentColor(Color color)
        {
            if (_customAccentDictionary == null)
            {
                _customAccentDictionary = [];
                Application.Current.Resources.MergedDictionaries.Add(_customAccentDictionary);
            }
            _customAccentDictionary["SystemAccentColor"] = color;
            _customAccentDictionary["SystemAccentColorLight1"] = color;
            _customAccentDictionary["SystemAccentColorLight2"] = color;
            _customAccentDictionary["SystemAccentColorLight3"] = color;
            _customAccentDictionary["SystemAccentColorDark1"] = color;
            _customAccentDictionary["SystemAccentColorDark2"] = color;
            _customAccentDictionary["SystemAccentColorDark3"] = color;
        }
        public static void RevertToSystemAccentColor()
        {
            if (_customAccentDictionary == null)
            {
                _customAccentDictionary = [];
                Application.Current.Resources.MergedDictionaries.Add(_customAccentDictionary);
            }
            _customAccentDictionary["SystemAccentColor"] = uiSettings.GetColorValue(UIColorType.Accent);
            _customAccentDictionary["SystemAccentColorLight1"] = uiSettings.GetColorValue(UIColorType.AccentLight1);
            _customAccentDictionary["SystemAccentColorLight2"] = uiSettings.GetColorValue(UIColorType.AccentLight2);
            _customAccentDictionary["SystemAccentColorLight3"] = uiSettings.GetColorValue(UIColorType.AccentLight3);
            _customAccentDictionary["SystemAccentColorDark1"] = uiSettings.GetColorValue(UIColorType.AccentDark1);
            _customAccentDictionary["SystemAccentColorDark2"] = uiSettings.GetColorValue(UIColorType.AccentDark2);
            Application.Current.Resources = customResourceDictionary;
        }
        public static void OverrideAccent()
        {
            try
            {
                if (Globals.InjectedCustomAccent == false)
                {
                    var customResources = new ResourceDictionary
                    {
                        Source = new Uri("ms-appx:///AppDesign/ThemeOverrides.xaml")
                    };
                    Application.Current.Resources.MergedDictionaries.Add(customResources);
                    Globals.InjectedCustomAccent = true;
                }
                switch (Globals.InjectCustomAccent)
                {
                    case 0:
                        RevertToSystemAccentColor();
                        break;
                    case 1:
                        SetAccentColor(Color.FromArgb(255, (byte)Globals.ColorSchemeSelectAccentR, (byte)Globals.ColorSchemeSelectAccentG, (byte)Globals.ColorSchemeSelectAccentB));
                        break;
                }
            }
            catch
            {
            }
        }
        public static void SetFont(FontFamily fontFamily)
        {
            var customResourceDictionary = Application.Current.Resources;
            customResourceDictionary["ContentControlThemeFontFamily"] = fontFamily;
            Application.Current.Resources = customResourceDictionary;
        }
        public static string CurrentFont()
        {
            var customResourceDictionary = Application.Current.Resources;
            return (customResourceDictionary["ContentControlThemeFontFamily"] as FontFamily).Source;
        }
        public static ElementTheme GetTheme()
        {
            return (CommonlyAccessedInstances.m_window.Content as FrameworkElement).ActualTheme;
        }
        public static void SetAppTheme(ElementTheme theme)
        {
            if (CommonlyAccessedInstances.m_window.Content is FrameworkElement frameworkElement)
            {
                frameworkElement.RequestedTheme = theme;
            }
        }
        public static void ImageOptionalBlur_Loaded()
        {
            var brush = (AcrylicBrush)Application.Current.Resources["CustomAcrylicInAppLuminosity"];
            var brush2 = (AcrylicBrush)Application.Current.Resources["CustomAcrylicInAppLuminositySlightTint"];
            var brush3 = (AcrylicBrush)Application.Current.Resources["ExpanderHeaderBackground"];
            brush.AlwaysUseFallback = Globals.useLessDemandingEffects;
            brush2.AlwaysUseFallback = brush.AlwaysUseFallback;
            brush3.AlwaysUseFallback = brush.AlwaysUseFallback;
        }
    }
}
