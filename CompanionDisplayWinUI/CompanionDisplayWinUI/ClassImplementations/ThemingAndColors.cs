using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using System;
using Windows.UI;
using Windows.UI.ViewManagement;

namespace CompanionDisplayWinUI.ClassImplementations
{
    static class ThemingAndColors
    {
        private static readonly ResourceDictionary customResourceDictionary = Application.Current.Resources;
        private static readonly UISettings uiSettings = new();
        public static void SetAccentColor(Color color)
        {
            var accentColor = customResourceDictionary["SystemAccentColor"];
            customResourceDictionary["SystemAccentColor"] = color;
            customResourceDictionary["SystemAccentColorLight1"] = accentColor;
            customResourceDictionary["SystemAccentColorLight2"] = accentColor;
            customResourceDictionary["SystemAccentColorLight3"] = accentColor;
            customResourceDictionary["SystemAccentColorDark1"] = accentColor;
            customResourceDictionary["SystemAccentColorDark2"] = accentColor;
            Application.Current.Resources = customResourceDictionary;
        }
        public static void RevertToSystemAccentColor()
        {
            customResourceDictionary["SystemAccentColor"] = uiSettings.GetColorValue(UIColorType.Accent);
            customResourceDictionary["SystemAccentColorLight1"] = uiSettings.GetColorValue(UIColorType.AccentLight1);
            customResourceDictionary["SystemAccentColorLight2"] = uiSettings.GetColorValue(UIColorType.AccentLight2);
            customResourceDictionary["SystemAccentColorLight3"] = uiSettings.GetColorValue(UIColorType.AccentLight3);
            customResourceDictionary["SystemAccentColorDark1"] = uiSettings.GetColorValue(UIColorType.AccentDark1);
            customResourceDictionary["SystemAccentColorDark2"] = uiSettings.GetColorValue(UIColorType.AccentDark2);
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
            brush.AlwaysUseFallback = Globals.useLessDemandingEffects;
            brush2.AlwaysUseFallback = brush.AlwaysUseFallback;
        }
    }
}
