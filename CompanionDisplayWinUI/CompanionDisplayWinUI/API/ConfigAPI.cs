using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using System;
using System.IO;

namespace CompanionDisplayWinUI.API
{
    static class ConfigAPI
    {
        public static void Save_Settings()
        {
            if(PageCacheHandler.homePage != null){
                PageCacheHandler.homePage.killThis();
                PageCacheHandler.homePage = null;
            }
            string settingsfile = Globals.ColorSchemeSelect + "\n" + Globals.InjectCustomAccent + "\n" + Globals.ColorSchemeSelectAccentR + "\n" + Globals.ColorSchemeSelectAccentG + "\n" + Globals.ColorSchemeSelectAccentB + "\n" + Globals.Backdrop + "\n" + Globals.BackgroundLink + "\n" + Globals.Wallpaper + "\n" + Globals.Blur + "\n" + Globals.StealFocus + "\n" + Globals.BackgroundColorR + "\n" + Globals.BackgroundColorG + "\n" + Globals.BackgroundColorB + "\n" + Globals.IsBetaProgram + "\n" + Globals.HideAddButton + "\n" + Globals.LaunchOnStartup + "\n" + Globals.LockLayout + "\n" + ThemingAPI.CurrentFont() + "\n" + Globals.sleepModeOpacity + "\n" + Globals.OverrideColor + "\n" + Globals.SleepColorR + "\n" + Globals.SleepColorG + "\n" + Globals.SleepColorB + "\n" + Globals.SearchEngine + "\n" + Globals.NewTabBehavior + "\n" + Globals.triggerSetup + "\n" + Globals.useLessDemandingEffects + "\n" + Globals.use12HourClock + "\n" + Globals.disableDiscord + "\n" + Globals.scale;
            File.WriteAllText("Config/GlobalSettings.crlh", settingsfile);
        }
        public static void Save_Settings_Background()
        {
            string settingsfile = Globals.enableUISounds + "\n" + Globals.showPromo + "\n" + Globals.IgnoreUpdates + "\n";
            File.WriteAllText("Config/SecSettings.crlh", settingsfile);
            ElementSoundPlayer.State = (ElementSoundPlayerState)(Convert.ToByte(Globals.enableUISounds) + 1);
        }
        public static void LoadGeneralConfigs()
        {
            try
            {
                Directory.CreateDirectory("Config");
                string WidgetOrder = File.ReadAllText("Config/GlobalSettings.crlh");
                using StringReader readerconfig = new(WidgetOrder);
                Globals.ColorSchemeSelect = int.Parse(readerconfig.ReadLine());
                Globals.InjectCustomAccent = int.Parse(readerconfig.ReadLine());
                Globals.ColorSchemeSelectAccentR = int.Parse(readerconfig.ReadLine());
                Globals.ColorSchemeSelectAccentG = int.Parse(readerconfig.ReadLine());
                Globals.ColorSchemeSelectAccentB = int.Parse(readerconfig.ReadLine());
                Globals.Backdrop = int.Parse(readerconfig.ReadLine());
                Globals.BackgroundLink = readerconfig.ReadLine();
                Globals.Wallpaper = readerconfig.ReadLine();
                Globals.Blur = bool.Parse(readerconfig.ReadLine());
                Globals.StealFocus = bool.Parse(readerconfig.ReadLine());
                Globals.BackgroundColorR = int.Parse(readerconfig.ReadLine());
                Globals.BackgroundColorG = int.Parse(readerconfig.ReadLine());
                Globals.BackgroundColorB = int.Parse(readerconfig.ReadLine());
                Globals.IsBetaProgram = bool.Parse(readerconfig.ReadLine());
                Globals.HideAddButton = bool.Parse(readerconfig.ReadLine());
                Globals.LaunchOnStartup = bool.Parse(readerconfig.ReadLine());
                Globals.LockLayout = bool.Parse(readerconfig.ReadLine());
                Globals.FontFamily = readerconfig.ReadLine();
                Globals.sleepModeOpacity = double.Parse(readerconfig.ReadLine());
                Globals.OverrideColor = bool.Parse(readerconfig.ReadLine());
                Globals.SleepColorR = int.Parse(readerconfig.ReadLine());
                Globals.SleepColorG = int.Parse(readerconfig.ReadLine());
                Globals.SleepColorB = int.Parse(readerconfig.ReadLine());
                try
                {
                    Globals.SearchEngine = new Uri(readerconfig.ReadLine());
                }
                catch
                {

                }
                Globals.NewTabBehavior = int.Parse(readerconfig.ReadLine());
                Globals.triggerSetup = bool.Parse(readerconfig.ReadLine());
                Globals.useLessDemandingEffects = bool.Parse(readerconfig.ReadLine());
                Globals.use12HourClock = bool.Parse(readerconfig.ReadLine());
                Globals.disableDiscord = bool.Parse(readerconfig.ReadLine());
                Globals.scale = float.Parse(readerconfig.ReadLine());
                readerconfig.Close();
            }
            catch
            {
            }
        }
        public static void LoadSecConfig(DispatcherQueue dispatcherQueue)
        {
            try
            {
                Directory.CreateDirectory("Config");
                string WidgetOrder = File.ReadAllText("Config/SecSettings.crlh");
                using StringReader readerconfig = new(WidgetOrder);
                Globals.enableUISounds = bool.Parse(readerconfig.ReadLine());
                dispatcherQueue.TryEnqueue(() =>
                {
                    ElementSoundPlayer.State = (ElementSoundPlayerState)(Convert.ToByte(Globals.enableUISounds) + 1);
                    ThemingAPI.ImageOptionalBlur_Loaded();
                });
                Globals.showPromo = bool.Parse(readerconfig.ReadLine());
                Globals.IgnoreUpdates = bool.Parse(readerconfig.ReadLine());
                readerconfig.Close();
            }
            catch
            {
            }
        }
        public static void LoadOBSConfig()
        {
            try
            {
                string OBSConfig = File.ReadAllText("Config/OBSSettings.crlh");
                using StringReader readerconfig1 = new(OBSConfig);
                Globals.obsIP = readerconfig1.ReadLine();
                Globals.obsPass = readerconfig1.ReadLine();
                readerconfig1.Close();
            }
            catch { }
        }
        public static void SaveOBSConfig()
        {
            string settingsfile = Globals.obsIP + "\n" + Globals.obsPass;
            File.WriteAllText(Globals.OBSConfigFile, settingsfile);
        }
        public static void LoadMusicConfig()
        {
            try
            {
                string PlayerConfig = File.ReadAllText(Globals.MusicSettingsFile);
                using StringReader readerconfig = new(PlayerConfig);
                Globals.MusicProvider = new Uri(readerconfig.ReadLine());
                Globals.obsPass = readerconfig.ReadLine();
                readerconfig.Close();
            }
            catch { }
        }
        public static void SaveMusicConfig()
        {
            string settingsfile = Globals.MusicProvider.ToString();
            File.WriteAllText(Globals.MusicSettingsFile, settingsfile);
        }
    }
}
