namespace CompanionDisplayWinUI.API
{
    static class ADBAPI
    {
        public static string ExecuteADBCommand(string command, string devID)
        {
            return CommandAPI.GetCMDLog("runtimes\\adb.exe - s " + devID + " " + command);
        }
        public static string ExecuteShellCommand(string command, string devID)
        {
            return ExecuteADBCommand("shell " + command, devID);
        }
        public static string GetBatteryProperty(string property, string devID)
        {
            return ExecuteShellCommand("dumpsys battery | findstr " + property, devID);
        }
        public static string GetDeviceName(string devID)
        {
            return ExecuteADBCommand("getprop ro.product.model", devID);
        }
        public static int GetDeviceBatteryLevel(string devID)
        {
            return int.Parse(GetBatteryProperty("level", devID).Replace(GetBatteryProperty("Capacity", devID), "")[9..]);
        }
        public static double GetDeviceBrightness(string devID)
        {
            return double.Parse(ExecuteShellCommand("settings get system screen_brightness", devID));
        }
    }
}
