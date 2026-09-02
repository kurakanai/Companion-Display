using CompanionDisplayWinUI.API;
using CompanionDisplayWinUI.Objects;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Threading;
using Windows.Foundation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI.ClassImplementations.SharedPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NORC_WidgetPhone : EmbeddedRCWidget
    {
        string LastID;
        public NORC_WidgetPhone()
        {
            this.InitializeComponent();
        }
        private void Frame_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            FrameworkElement senderElement = sender as FrameworkElement;
            MenuFlyoutItem[] flyoutRC =
            [
                new(){ Text = AppStrings.removeWidget, Name = senderElement.Name + "Flyout" },
                new(){ Text = AppStrings.widgetRefresh, Name = senderElement.Name + "Edit" },
                new(){ Text = AppStrings.widgetPinUnpin, Name = senderElement.Name + "Pin" },
                new(){ Text = AppStrings.pipOpen, Name = senderElement.Name + "PiP" },
            ];
            flyoutRC[0].Click += MenuFlyoutItem_Click;
            flyoutRC[1].Click += MenuFlyoutEdit_Click;
            flyoutRC[2].Click += PinButton;
            flyoutRC[3].Click += PiPButton;
            TriggerRightClick(flyoutRC, senderElement);
        }

        private void PiPButton(object sender, RoutedEventArgs e)
        {
            var frame = this.Parent as CommonWidgetContainer;
            frame.TriggerRightClickFromChild("pip");
        }
        private void PinButton(object sender, RoutedEventArgs e)
        {
            var frame = this.Parent as CommonWidgetContainer;
            frame.TriggerRightClickFromChild("pin");
        }
        private void MenuFlyoutEdit_Click(object sender, RoutedEventArgs e)
        {
            BasicGridView.Items.Clear();
            Thread thread0 = new(UpdateUI);
            thread0.Start();
        }
        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            var frame = this.Parent as CommonWidgetContainer;
            frame.TriggerRightClickFromChild("");
        }

        private void MainGrid_Loaded(object sender, RoutedEventArgs e)
        {
            BasicGridView.Items.Clear();
            Thread thread0 = new(UpdateUI);
            thread0.Start();
        }

        private void UpdateUI()
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                try
                {
                    string output = CommandAPI.GetCMDLog("runtimes\\adb.exe devices").Replace("List of devices attached", "").Replace("\tdevice", "");
                    foreach (string line in output.Split('\n'))
                    {
                        try
                        {
                            string fix = line.Replace("\r", "");
                            if (fix.Length != 0)
                            {
                                LastID = fix;
                                Frame frame = new()
                                {
                                    Name = fix,
                                    Width = 240,
                                };
                                BasicGridView.Items.Add(frame);
                                frame.Navigate(typeof(WidgetPhoneIndividual));
                            }
                        }
                        catch { }
                    }
                    if (BasicGridView.Items.Count == 1)
                    {
                        var childControl = (Microsoft.UI.Xaml.Controls.Frame)BasicGridView.FindName(LastID);
                        childControl.Width = 486;
                    }
                    NoDevices.Visibility = (Visibility)Convert.ToByte(!(BasicGridView.Items.Count == 0));
                }
                catch { }
            });
        }
    }
}
