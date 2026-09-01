using System;

namespace CompanionDisplayWinUI.API
{
    static class ADBAPI
    {
        public static string executeADBCommand(string command, string devID)
        {
            return CommandAPI.GetCMDLog("runtimes\\adb.exe - s " + devID + " " + command);
        }
        public static string executeShellCommand(string command, string devID)
        {
            return executeADBCommand("shell " + command, devID);
        }
        public static string getBatteryProperty(string property, string devID)
        {
            return executeShellCommand("dumpsys battery | findstr " + property, devID);
        }
        public static string getDeviceName(string devID)
        {
            return executeADBCommand("getprop ro.product.model", devID);
        }
        public static int getDeviceBatteryLevel(string devID)
        {
            return int.Parse(getBatteryProperty("level", devID).Replace(getBatteryProperty("Capacity", devID), "")[9..]);
        }
        public static double getDeviceBrightness(string devID)
        {
            return Double.Parse(executeShellCommand("settings get system screen_brightness", devID));
        }
    }
}
