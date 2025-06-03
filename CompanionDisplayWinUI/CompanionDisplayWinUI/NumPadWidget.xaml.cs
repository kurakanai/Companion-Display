using CompanionDisplayWinUI.ClassImplementations;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using System.Threading;
using Windows.System;
using Windows.UI.Core;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NumPadWidget : Page
    {
        public NumPadWidget()
        {
            this.InitializeComponent();
        }
        public bool CleanUp = false;
        private CoreVirtualKeyStates LastNumKeyState = Windows.UI.Core.CoreVirtualKeyStates.None;
        private Windows.UI.Core.CoreVirtualKeyStates numkey;
        private void UpdateUI()
        {
            numkey = Microsoft.UI.Input.InputKeyboardSource.GetKeyStateForCurrentThread(Windows.System.VirtualKey.NumberKeyLock);
            if(numkey != LastNumKeyState)
            {
                LastNumKeyState = numkey;
                if (numkey != Windows.UI.Core.CoreVirtualKeyStates.Locked)
                {
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        NumLock.IsChecked = false;
                        Numpad0.Content = "Ins";
                        Numpad1.Content = "End";
                        Numpad2.Content = "↓";
                        Numpad3.Content = "PgDn";
                        Numpad4.Content = "←";
                        Numpad5.Content = "";
                        Numpad6.Content = "→";
                        Numpad7.Content = "Home";
                        Numpad8.Content = "↑";
                        Numpad9.Content = "PgUp";
                        NumpadDot.Content = "Del";
                    });
                }
                else
                {
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        NumLock.IsChecked = true;
                        Numpad0.Content = "0";
                        Numpad1.Content = "1";
                        Numpad2.Content = "2";
                        Numpad3.Content = "3";
                        Numpad4.Content = "4";
                        Numpad5.Content = "5";
                        Numpad6.Content = "6";
                        Numpad7.Content = "7";
                        Numpad8.Content = "8";
                        Numpad9.Content = "9";
                        NumpadDot.Content = ".";
                    });
                }
            }
            if(!CleanUp)
            {
                Thread.Sleep(300);
                Thread thread = new(UpdateUI);
                thread.Start();
            }
        }
        private void PressKey(object sender, RoutedEventArgs e)
        {
            Windows.UI.Core.CoreVirtualKeyStates numkey = Microsoft.UI.Input.InputKeyboardSource.GetKeyStateForCurrentThread(Windows.System.VirtualKey.NumberKeyLock);
            try
            {
                if (numkey != Windows.UI.Core.CoreVirtualKeyStates.Locked)
                {
                    KeyPressAPI.PressKey((VirtualKey)int.Parse(((RepeatButton)sender).DataContext.ToString()), up: false);
                    KeyPressAPI.PressKey((VirtualKey)int.Parse(((RepeatButton)sender).DataContext.ToString()), up: true);
                }
                else
                {
                    KeyPressAPI.PressKey((VirtualKey)int.Parse(((RepeatButton)sender).Tag.ToString()), up: false);
                    KeyPressAPI.PressKey((VirtualKey)int.Parse(((RepeatButton)sender).Tag.ToString()), up: true);
                }
            }
            catch
            {
                KeyPressAPI.PressKey((VirtualKey)int.Parse(((ToggleButton)sender).Tag.ToString()), up: false);
                KeyPressAPI.PressKey((VirtualKey)int.Parse(((ToggleButton)sender).Tag.ToString()), up: true);
            }
        }
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            CleanUp = true;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CleanUp = false;
            Thread thread = new(UpdateUI);
            thread.Start();
        }
    }
}
