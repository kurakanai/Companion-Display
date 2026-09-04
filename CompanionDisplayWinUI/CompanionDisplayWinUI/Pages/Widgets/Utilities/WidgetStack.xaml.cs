using CompanionDisplayWinUI.API;
using CompanionDisplayWinUI.ClassImplementations;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System;
using System.IO;
using System.Threading;
using Windows.Foundation;
using Windows.UI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WidgetStack : Page
    {
        public WidgetStack()
        {
            this.InitializeComponent();
        }
        private string WidgetOrder = "";

        private void UpdateUI()
        {
            WidgetAPI.AddWidgets(WidgetStackView, WidgetOrder);
            DispatcherQueue.TryEnqueue(() =>
            {
                Frame grid = new()
                {
                    Height = 300,
                    Width = 500,
                    CornerRadius = new CornerRadius(10),
                    Background = (AcrylicBrush)Application.Current.Resources["CustomAcrylicInAppLuminosity"],
                };
                Button button = new()
                {
                    Name = "AddWidget",
                    Content = AppStrings.addToStack,
                    FontFamily = new FontFamily("Segoe UI Variable Display Light"),
                    Height = 300,
                    Width = 500,
                    FontSize = 36,
                    Margin = new Thickness(0),
                    CornerRadius = new CornerRadius(10),
                    BorderThickness = new Thickness(0),
                    Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)),
                    AllowDrop = false,
                };
                addBtn = button;
                grid.Content = button;
                button.Click += Button_Click;
                grid.RightTapped += Frame_RightTapped;
                button.DropCompleted += Button_DropCompleted;
                WidgetStackView.Items.Add(grid);
            });
        }

        Button addBtn;
        private void Button_DropCompleted(UIElement sender, DropCompletedEventArgs args)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Globals.IsAllApps = true;
                var frame = (((((this.Frame.Parent as GridView).Parent as ScrollViewer).Parent as Grid).Parent as Page).Parent as Frame);
                var navviewparent = frame.Parent as NavigationView;
                navviewparent.Tag = this.Frame.Name;
                frame.Navigate(typeof(AllWidgets));
                navviewparent.SelectedItem = -1;
            }
            catch
            {
                Globals.IsAllApps = false;
            }
        }

        private void Frame_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if ((sender as Frame).Parent as FlipView != null)
            {
                FrameworkElement senderElement = sender as FrameworkElement;
                MenuFlyout myFlyout = new();
                MenuFlyoutItem firstItem = new() { Text = AppStrings.removeStack, Name = senderElement.Name + "StackRemove" };
                Frame frame = senderElement as Frame;
                Type type1 = Type.GetType(frame.Content.ToString());
                if (type1 != null)
                {
                    MenuFlyoutItem secondItem = new() { Text = AppStrings.openStackItemMenu, Name = senderElement.Name + "ItemOpen", Tag = sender };
                    secondItem.Click += MenuFlyoutEdit_Click;
                    myFlyout.Items.Add(secondItem);
                }
                MenuFlyoutItem thirdItem = new() { Text = AppStrings.widgetPinUnpin, Name = senderElement.Name + "Pin" };
                MenuFlyoutItem fourthItem = new() { Text = AppStrings.pipOpen, Name = senderElement.Name + "PiP" };
                thirdItem.Click += PinButton;
                fourthItem.Click += PiPButton;
                Frame frame3 = senderElement as Frame;
                firstItem.Click += MenuFlyoutItem_Click;
                myFlyout.Items.Add(firstItem);
                myFlyout.Items.Add(thirdItem);
                myFlyout.Items.Add(fourthItem);
                myFlyout.ShowAt(senderElement, new Point(0, 0));
            }
        }
        private void PiPButton(object sender, RoutedEventArgs e)
        {
            var frame = this.Frame;
            frame.Tag = "pip";
            frame.IsEnabled = false;
            frame.IsEnabled = true;
        }

        private void PinButton(object sender, RoutedEventArgs e)
        {
            var frame = this.Frame;
            frame.Tag = "pin";
            frame.IsEnabled = false;
            frame.IsEnabled = true;
        }
        private void Frame_RightTappedActions(object sender, RightTappedRoutedEventArgs _1)
        {
            if ((sender as Frame).Parent as FlipView != null)
            {
                FrameworkElement senderElement = sender as FrameworkElement;
                MenuFlyout myFlyout = new();
                MenuFlyoutItem firstItem = new() { Text = AppStrings.widgetRemove, Name = senderElement.Name + "Flyout" };
                Frame frame = senderElement as Frame;
                Type type1 = Type.GetType(frame.Content.ToString() + "WidgetSettings");
                if (type1 != null)
                {
                    MenuFlyoutItem secondItem = new() { Text = AppStrings.widgetEdit, Name = senderElement.Name + "Edit" };
                    secondItem.Click += EditItem;
                    myFlyout.Items.Add(secondItem);
                }
                Frame frame3 = senderElement as Frame;
                firstItem.Click += RemoveWidget;
                myFlyout.Items.Add(firstItem);
                myFlyout.ShowAt(senderElement, new Point(0, 0));
            }
        }

        private void EditItem(object sender, RoutedEventArgs e)
        {
            var senderElement = sender as MenuFlyoutItem;
            var childControl = (Microsoft.UI.Xaml.Controls.Frame)WidgetStackView.FindName(senderElement.Name[..^4]);
            var frame = childControl as Frame;
            Type type1 = Type.GetType(frame.Content + "WidgetSettings");
            frame.Navigate(type1);
        }
        private void SaveTo()
        {
            if (Globals.IsAllApps == false)
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    var Items = WidgetStackView.Items;
                    string Order = "";
                    foreach (var widget in Items)
                    {
                        try
                        {
                            var item = widget as Frame;
                            string deedify = item.Content.ToString().Replace("WidgetSettings", "");
                            if (!deedify.Contains("CompanionDisplayWinUI.UpdateWarning") && !deedify.Contains("Microsoft.UI.Xaml.Controls.Button"))
                            {
                                Order = deedify switch
                                {
                                    var s when (deedify.Contains("CompanionDisplayWinUI.NotesWidget") || deedify.Contains("CompanionDisplayWinUI.WidgetStack")) => Order + deedify + "ID" + item.Tag.ToString() + Environment.NewLine,
                                    var s when deedify.Contains("CompanionDisplayWinUI.WidgetPhoto") => Order + deedify + "IMAGESOURCE" + item.Tag.ToString() + Environment.NewLine,
                                    _ => Order + deedify + Environment.NewLine,
                                };
                            }
                        }
                        catch
                        {
                        }
                        File.WriteAllText("Config/Stacks/" + this.Frame.Name.ToString() + ".crlh", Order);
                    }
                });
            }
        }
        private void RemoveWidget(object sender, RoutedEventArgs e)
        {
            FrameworkElement senderElement = sender as FrameworkElement;
            var childControl = (Microsoft.UI.Xaml.Controls.Frame)WidgetStackView.FindName(senderElement.Name[..^6]);
            WidgetStackView.Items.Remove(childControl);
            try
            {
                File.Delete("Config/Stacks/" + this.Frame.Tag.ToString() + ".crlh");
            }
            catch { }
            Thread thread = new(SaveTo);
            thread.Start();
        }

        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                File.Delete("Config/Stacks/" + this.Frame.Tag.ToString() + ".crlh");
            }
            catch { }
            this.Frame.Tag = "";
            this.Frame.IsEnabled = false;
            this.Frame.IsEnabled = true;
        }

        private void MenuFlyoutEdit_Click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem menuFlyoutItem = sender as MenuFlyoutItem;
            Frame_RightTappedActions(menuFlyoutItem.Tag as Frame, null);
        }

        private void BugcheckAcrylic(object _1, RoutedEventArgs _2)
        {
            
        }
        private void Frame_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs _1)
        {
            switch ((sender as Frame).Tag)
            {
                default:
                    WidgetStackView.Items.Remove(sender);
                    Thread thread = new(SaveTo);
                    thread.Start();
                    break;
            }
        }


        private void Frame_IsEnabledChanged2(object _1, DependencyPropertyChangedEventArgs _2)
        {
            
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                WidgetOrder = File.ReadAllText("Config/Stacks/" + (this.Frame).Name.ToString() + ".crlh");
                WidgetStackView.Items.Clear();
            }
            catch
            {
                try
                {
                    Directory.CreateDirectory("Config/Stacks");
                    File.WriteAllText("Config/Stacks/" + (this.Frame).Name.ToString() + ".crlh", "");
                }
                catch
                {

                }
            }
            Thread thread = new(UpdateUI);
            thread.Start();
        }
    }
}
