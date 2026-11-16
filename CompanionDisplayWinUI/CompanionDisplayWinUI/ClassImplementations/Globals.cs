using CompanionDisplayWinUI.API;
using LibreHardwareMonitor.Hardware;
using Microsoft.Web.WebView2.Core;
using System;

namespace CompanionDisplayWinUI.ClassImplementations
{
    static class Globals
    {
        // Runtime variables
        public static bool IsAdmin = false;
        public static string Version = "26.11";
        public static ObsControls obsControls = new();
        public static SleepTimer sleepTimer = new();
        public static ServerComponent serverComponent = new();
        public static CoreWebView2 CoreWebView2 = null;
        public static string UpdateZip = "https://github.com/kurakanai/Companion-Display/releases/latest/download/release.zip";
        public static string UpdateZipSig = "https://github.com/kurakanai/Companion-Display/releases/latest/download/release.zip.sig";
        public static string UpdateString = "https://github.com/kurakanai/Companion-Display/releases/latest/download/release.txt";
        public static string UpdateZipBeta = "https://github.com/kurakanai/Companion-Display/releases/download/prerelease_latest/release.zip";
        public static string UpdateZipSigBeta = "https://github.com/kurakanai/Companion-Display/releases/download/prerelease_latest/release.zip.sig";
        public static string UpdateStringBeta = "https://github.com/kurakanai/Companion-Display/releases/download/prerelease_latest/release.txt";
        public static string PublicKeyPem = """
        -----BEGIN PUBLIC KEY-----
        MFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAEVsK3QUj2Dpn6mS9YINAZ5O7S5PH4
        Fpz0T6leOyqjxhIXWZ5SpoP7UfTti7qJLOBx5x/2Hfa0EVRP75fQ6akr3Q==
        -----END PUBLIC KEY-----
        """;
        public static string DiscordID = "1155340829485961277";
        public static string obsIP = "ws://127.0.0.1:4455";
        public static string obsPass = "";
        public static string SongID = "";
        public static bool ClearTab = false;
        public static bool StartedPlayer = false;
        public static bool IsAllApps = false;
        public static bool InjectedCustomAccent = false;
        public static bool ImageFailed = false;
        public static IHardware CurrentHW;
        public static TimeAPI publicTimeViewModel = new();
        public static int PiPAmount = 0;
        public static bool injectedSizeChangeEvent = false;

        // App config files
        public static string RefreshTokenPath = "Config/RefreshToken.crlh";
        public static string RefreshToken2Path = "Config/RefreshToken2.crlh";
        public static string MediaConfigFile = "Config/MediaConfig.crlh";
        public static string TimeConfigFile = "Config/TimeConfig.crlh";
        public static string TimeConfigFileQS = "Config/TimeConfigQS.crlh";
        public static string PhotoConfigFile = "Config/PhotoConfig.crlh";
        public static string OBSConfigFile = "Config/OBSSettings.crlh";
        public static string MusicSettingsFile = "Config/PlayerSettings.crlh";
        public static string WidgetOrderFile = "Config/WidgetOrder.crlh";
        public static string BlackBoxFile = "Config/BlackBox.crlh";

        // Loaded Configs
        public static int ColorSchemeSelect = 0;
        public static int ColorSchemeSelectAccentR = 0;
        public static int ColorSchemeSelectAccentG = 0;
        public static int ColorSchemeSelectAccentB = 0;
        public static int BackgroundColorR = 0;
        public static int BackgroundColorG = 0;
        public static int BackgroundColorB = 0;
        public static int InjectCustomAccent = 0;
        public static int Backdrop = 0;
        public static string BackgroundLink = "";
        public static string Wallpaper = "";
        public static string FontFamily = "";
        public static bool Blur = true;
        public static bool StealFocus = false;
        public static bool IgnoreUpdates = false;
        public static bool IsUpdateAvailable = false;
        public static bool SmartFlipToggle = false;
        public static bool IsBetaProgram = false;
        public static bool HideAddButton = false;
        public static bool LaunchOnStartup = false;
        public static bool LockLayout = false;
        public static double sleepModeOpacity = 10;
        public static bool OverrideColor = false;
        public static int SleepColorR = 0;
        public static int SleepColorG = 0;
        public static int SleepColorB = 0;
        public static int NewTabBehavior = 0;
        public static bool enableUISounds = false;
        public static Uri SearchEngine = new("https://www.google.com");
        public static Uri MusicProvider = new("https://open.spotify.com");

        public static bool triggerSetup = true;
        public static bool useLessDemandingEffects = false;
        public static bool use12HourClock = false;
        public static bool showPromo = true;
        public static bool disableDiscord = false;
        public static float scale = 1.0f;
    }
}
