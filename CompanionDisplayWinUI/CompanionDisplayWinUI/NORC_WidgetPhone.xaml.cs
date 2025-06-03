using Windows.Foundation;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System.Threading;
using CompanionDisplayWinUI.ClassImplementations;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NORC_WidgetPhone : Page
    {
        string LastID;
        public NORC_WidgetPhone()
        {
            this.InitializeComponent();
        }
        private void Frame_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if ((this.Parent as Frame).Parent as Grid == null)
            {
                FrameworkElement senderElement = sender as FrameworkElement;
                MenuFlyout myFlyout = new();
                MenuFlyoutItem firstItem = new() { Text = AppStrings.removeWidget, Name = senderElement.Name + "Flyout" };
                MenuFlyoutItem secondItem = new() { Text = AppStrings.widgetRefresh, Name = senderElement.Name + "Edit" };
                MenuFlyoutItem thirdItem = new() { Text = AppStrings.widgetPinUnpin, Name = senderElement.Name + "Pin" };
                MenuFlyoutItem fourthItem = new() { Text = AppStrings.pipOpen, Name = senderElement.Name + "PiP" };
                firstItem.Click += MenuFlyoutItem_Click;
                secondItem.Click += MenuFlyoutEdit_Click;
                thirdItem.Click += PinButton;
                fourthItem.Click += PiPButton;
                myFlyout.Items.Add(firstItem);
                myFlyout.Items.Add(secondItem);
                if (this.Frame.Parent is not FlipView)
                {
                    myFlyout.Items.Add(thirdItem);
                    myFlyout.Items.Add(fourthItem);
                }
                myFlyout.ShowAt(senderElement, new Point(0, 0));
            }
        }

        private void PinButton(object sender, RoutedEventArgs e)
        {
            var frame = this.Parent as Frame;
            frame.Tag = "pin";
            frame.IsEnabled = false;
            frame.IsEnabled = true;
        }
        private void PiPButton(object sender, RoutedEventArgs e)
        {
            var frame = this.Parent as Frame;
            frame.Tag = "pip";
            frame.IsEnabled = false;
            frame.IsEnabled = true;
        }
        private void MenuFlyoutEdit_Click(object sender, RoutedEventArgs e)
        {
            BasicGridView.Items.Clear();
            Thread thread0 = new(UpdateUI);
            thread0.Start();
        }
        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            var frame = this.Parent as Frame;
            frame.Tag = "";
            frame.IsEnabled = false;
            frame.IsEnabled = true;
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
                    string output = CMDOperations.GetCMDLog("runtimes\\adb.exe devices").Replace("List of devices attached", "").Replace("\tdevice", "");
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
