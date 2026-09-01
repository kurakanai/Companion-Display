using Microsoft.Windows.ApplicationModel.Resources;
namespace CompanionDisplayWinUI.ClassImplementations
{
    static class AppStrings
    {
        private static readonly ResourceLoader resourceLoader = new();
        // Windowing
        public static string fullscrenEnter = resourceLoader.GetString("FullScreenEnter");
        public static string fullscrenExit = resourceLoader.GetString("FullScreenExit");
        // Backup Feature
        public static string backupFilters = resourceLoader.GetString("backupFilters");
        public static string backupTitleBar = resourceLoader.GetString("backupTitleBar");
        public static string backupPerformString = resourceLoader.GetString("backupPerformString");
        public static string restoringString = resourceLoader.GetString("restoringString");
        public static string backupString = resourceLoader.GetString("backupString");
        public static string restoreString = resourceLoader.GetString("restoreString");
        public static string restoreHeaderString = resourceLoader.GetString("restoreHeaderString");
        public static string backupHeaderString = resourceLoader.GetString("backupHeaderString");
        // App Configuration Files
        public static string configTypeGeneralSettings = resourceLoader.GetString("configTypeGeneralSettings");
        public static string configTypeWidgetLayout = resourceLoader.GetString("configTypeWidgetLayout");
        public static string configTypeWidgetSettings = resourceLoader.GetString("configTypeWidgetSettings");
        public static string configTypeBrowserData = resourceLoader.GetString("configTypeBrowserData");
        // Generic Operations
        public static string cancelString = resourceLoader.GetString("cancelString");
        // Volume Mixer
        public static string volumeMaster = resourceLoader.GetString("MasterVolume");
        public static string volumeSystem = resourceLoader.GetString("SystemSounds");
        // Update Strings
        public static string updateUpdate = resourceLoader.GetString("Update2");
        public static string updateUpToDate = resourceLoader.GetString("UpToDate");
        public static string updateUpdating = resourceLoader.GetString("Updating");
        public static string UpdateVersion(string version) {
            return (resourceLoader.GetString("UpdateString") + " " + version);
        }
        // OBS Strings
        public static string obsConnected = resourceLoader.GetString("StatusConnected");
        public static string obsDisconnected = resourceLoader.GetString("StatusDisconnected");
        public static string obsConnectionFailed = resourceLoader.GetString("ConnectionFailed");
        public static string obsReconnect = resourceLoader.GetString("Reconnect");
        // Shortcuts
        public static string appShortcutDescription = resourceLoader.GetString("StartupObject");
        // Developer info
        public static string devGithubUrl = "https://github.com/kurakanai";
        public static string devPaypalUrl = "https://www.paypal.com/paypalme/dinisp25";
        // PiP
        public static string pipIsOpen = resourceLoader.GetString("PiPOpen");
        // Widget Flyouts
        public static string widgetRemove = resourceLoader.GetString("Remove");
        public static string removeWidget = resourceLoader.GetString("RemoveWidget");
        public static string removeAllShortcuts = resourceLoader.GetString("ShortcutsRemoveAll");
        public static string widgetReplace = resourceLoader.GetString("Replace");
        public static string widgetRefresh = resourceLoader.GetString("Refresh");
        public static string widgetEdit = resourceLoader.GetString("Edit");
        public static string widgetPin = resourceLoader.GetString("Pin");
        public static string widgetPinUnpin = resourceLoader.GetString("PinToggle");
        public static string widgetUnpin = resourceLoader.GetString("Unpin");
        public static string pipOpen = "Picture in Picture";
        // Browser
        public static string browserLaunchpadAdd = resourceLoader.GetString("AddLaunchpadTile");
        public static string browserLaunchpadFlyoutAdd = resourceLoader.GetString("Add");
        public static string browserLaunchpadURLTemplate = resourceLoader.GetString("InsertURLHere");
        public static string browserNewTab = resourceLoader.GetString("NewTab");
        public static string browserLaunchpadTab = resourceLoader.GetString("Launchpad2");
        public static string browserLaunchpadEdit = widgetEdit;
        public static string browserLaunchpadRemove = widgetRemove;
        // Sensors
        public static string sensorsFreq = " MHz";
        public static string sensorsRPM = " RPM";
        public static string sensorsLoad = " %";
        public static string sensorsPower = " W";
        public static string sensorsVoltage = " V";
        public static string sensorsSmallData = " MB";
        public static string sensorsTemperature = " ºC";
        // Devices
        public static string adbPhoneNameError = resourceLoader.GetString("Unknown");
        // Launchpad
        public static string launchpadReplace = resourceLoader.GetString("Replace");
        public static string launchpadRemove = widgetRemove;
        // Sleep Timer
        public static string sleepTimerEnd = resourceLoader.GetString("EndSleepTimer");
        public static string sleepTimerAlreadyActive = resourceLoader.GetString("SleepTimerAlreadyActive");
        public static string sleepTimer = resourceLoader.GetString("SleepTimer");
        public static string sleepTimerStart = resourceLoader.GetString("StartSleepTimer");
        public static string sleepTimerMinsPlaceholder = resourceLoader.GetString("MinsPlaceholder");
        // Media
        public static string mediaNoMedia = resourceLoader.GetString("NoSongPlaying");
        public static string mediaPlaceholderTime = "--:--";
        public static string mediaNoLyrics = resourceLoader.GetString("NoLyricsAvailable");
        // Photos
        public static string replaceImage = resourceLoader.GetString("ReplaceImage");
        public static string removeImage = resourceLoader.GetString("RemoveImage");
        // OBS
        public static string start = resourceLoader.GetString("Start");
        public static string stop = resourceLoader.GetString("Stop");
        public static string doYouWanna = resourceLoader.GetString("DoYouWant");
        public static string confirmOBS = resourceLoader.GetString("ConfirmOBS");
        // Bool Stuff
        public static string trueString = resourceLoader.GetString("TrueString");
        public static string falseString = resourceLoader.GetString("FalseString");
        // Permission Stuff
        public static string yourPC = resourceLoader.GetString("yourPC");
        // Stacks
        public static string addToStack = resourceLoader.GetString("AddToStack");
        public static string removeStack = resourceLoader.GetString("RemoveStack");
        public static string openStackItemMenu = resourceLoader.GetString("OpenStackItemMenu");
        // PC Actions
        public static string GetActionString(string name)
        {
            return resourceLoader.GetString(name);
        }
    }
}
