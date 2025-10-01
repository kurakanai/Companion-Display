using System;
using System.Runtime.InteropServices;
using Windows.System;
using Microsoft.UI.Xaml.Input;
using System.Windows.Controls;

namespace CompanionDisplayWinUI.ClassImplementations
{
    public static class KeyPressAPI
    {
        [DllImport("user32.dll", SetLastError = true)]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);
        public static void PressKey(VirtualKey key, bool up)
        {
            const int KEYEVENTF_EXTENDEDKEY = 0x1;
            const int KEYEVENTF_KEYUP = 0x2;
            if (up)
                keybd_event((byte)key, 0x45, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, (UIntPtr)0);
            else
                keybd_event((byte)key, 0x45, KEYEVENTF_EXTENDEDKEY, (UIntPtr)0);
        }
        public static void callKeys(int desiredKey, int secondaryKey)
        {
            PressKey((VirtualKey)secondaryKey, up: false);
            PressKey((VirtualKey)desiredKey, up: false);
            PressKey((VirtualKey)desiredKey, up: true);
            PressKey((VirtualKey)secondaryKey, up: true);
        }
        public static void PressKeyCTRL(object sender, TappedRoutedEventArgs e)
        {
            callKeys(int.Parse((string)(sender as Button).Tag), 17);
        }

        public static void PressKeyNoModifiers(object sender, TappedRoutedEventArgs e)
        {
            callKeys(int.Parse((string)(sender as Button).Tag), -1);
        }
    }
}
