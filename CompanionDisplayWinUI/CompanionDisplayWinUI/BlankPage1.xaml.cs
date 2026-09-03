using CompanionDisplayWinUI.API;
using CompanionDisplayWinUI.ClassImplementations;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.UI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BlankPage1 : Page
    {
        public BlankPage1()
        {
            this.InitializeComponent();
            PageCacheHandler.homePage = this;
            CommonlyAccessedInstances.blankPage1 = this;
            CommonlyAccessedInstances.BasicGridView = BasicGridView;
            CommonlyAccessedInstances.PinnedView = PinnedView;
            this.NavigationCacheMode = NavigationCacheMode.Required;
            Thread thread0 = new(UpdateUI);
            thread0.Start();
        }
        private void GridView_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            _ = DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, () =>
            {
                // Set the drag data
                var page = sender as GridView;
                e.Data.SetData("sourceGridView", page.Tag);
            });
        }

        private void GridView_DragOver(object sender, DragEventArgs e)
        {
            try
            {
                _ = DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, () =>
                {
                    e.AcceptedOperation = DataPackageOperation.Copy;
                    var sourceGridView = e.DataView.Properties["sourceGridView"] as string;
                    if (sourceGridView != (sender as GridView).Tag as string)
                    {
                        e.AcceptedOperation = DataPackageOperation.None;
                    }
                });
            }
            catch { }
        }

        private void GridView_Drop(object sender, DragEventArgs e)
        {
            _ = DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, () =>
            {
                var sourceGridView = e.DataView.Properties["sourceGridView"] as string;
                var targetGridView = sender as GridView;
                if (sourceGridView == targetGridView.Tag as string)
                {
                    var items = e.DataView.GetDataAsync("Text").GetResults() as IEnumerable<string>;
                    foreach (var item in items)
                    {
                        (targetGridView.ItemsSource as IList<string>).Add(item);
                    }
                    Thread thread = new(SaveTo);
                    thread.Start();
                }
            });
        }
        public void SaveTo()
        {
            if(!Globals.IsAllApps)
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    SavingAPI.SaveGridLayout(BasicGridView, "Config/WidgetOrder.crlh");
                    SavingAPI.SaveGridLayout(PinnedView, "Config/PinnedOrder.crlh");
                });
            }
        }
        Button addBtn;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Globals.IsAllApps = true;
                var frame = this.Parent as Frame;
                var navviewparent = frame.Parent as NavigationView;
                frame.Navigate(typeof(AllWidgets));
                navviewparent.SelectedItem = -1;
            }
            catch
            {
                Globals.IsAllApps = false;
            }
        }

        public void Frame_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if((sender as Frame).Parent as GridView != null)
            {
                FrameworkElement senderElement = sender as FrameworkElement;
                MenuFlyout myFlyout = new();
                MenuFlyoutItem firstItem = new() { Tag = sender as Frame, Text = AppStrings.widgetRemove, Name = senderElement.Name + "Flyout" };
                Frame frame = senderElement as Frame;
                Type type1 = Type.GetType(frame.Content.ToString() + "WidgetSettings");
                if (type1 != null)
                {
                    MenuFlyoutItem secondItem = new() { Tag = sender as Frame, Text = AppStrings.widgetEdit, Name = senderElement.Name + "Edit" };
                    secondItem.Click += MenuFlyoutEdit_Click;
                    myFlyout.Items.Add(secondItem);
                }
                MenuFlyoutItem thirdItem = new() { Tag = sender as Frame, Text = AppStrings.widgetPin, Name = senderElement.Name + "Flyout" };
                if (senderElement.Parent == PinnedView)
                {
                    thirdItem.Text = AppStrings.widgetUnpin;
                }
                MenuFlyoutItem fourthItem = new() { Tag = sender as Frame, Text = AppStrings.pipOpen, Name = senderElement.Name + "PiP" };
                Frame frame3 = senderElement as Frame;
                firstItem.Click += MenuFlyoutItem_Click;
                thirdItem.Tag = sender as Frame;
                thirdItem.Click += Pin_Click;
                fourthItem.Tag = sender as Frame;
                fourthItem.Click += OpenPiP;
                myFlyout.Items.Add(firstItem);
                myFlyout.Items.Add(thirdItem);
                myFlyout.Items.Add(fourthItem);
                myFlyout.ShowAt(senderElement, new Point(0, 0));
            }
        }

        public void OpenPiP(object sender, RoutedEventArgs e)
        {
            Frame darkframe = (sender as MenuFlyoutItem).Tag as Frame;
            gridViewPiPTemp = darkframe.Parent as GridView;
            if (gridViewPiPTemp != null)
            {
                indexTemp = gridViewPiPTemp.Items.IndexOf(darkframe);
                darkframe.Unloaded += LoadWindowStage2;
                gridViewPiPTemp.Items.RemoveAt(indexTemp);
            }
        }
        public int indexTemp = 0;
        GridView gridViewPiPTemp;
        private void LoadWindowStage2(object sender, RoutedEventArgs e)
        {
            (sender as Frame).Unloaded -= LoadWindowStage2;
            Grid placeholder = new()
            {
                Height = 300,
                Width = 500
            };
            TextBlock textBlock = new()
            {
                Text = AppStrings.pipIsOpen,
                FontSize = 20,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            placeholder.Children.Add(textBlock);
            gridViewPiPTemp.Items.Insert(indexTemp, placeholder);
            PopupWidget m_window = new(sender as Frame, gridViewPiPTemp, placeholder);
            m_window.Closed += (s, e) =>
            {
                Globals.PiPAmount--;
                addBtn.IsEnabled = Globals.PiPAmount == 0;
            };
            m_window.Activate();
            Globals.PiPAmount++;
            addBtn.IsEnabled = false;
        }

        public void Pin_Click_NC(object sender, RoutedEventArgs _1)
        {
            if ((sender as Frame).Parent == BasicGridView)
            {
                BasicGridView.Items.Remove(sender);
                PinnedView.Items.Add(sender);
            }
            else
            {
                PinnedView.Items.Remove(sender);
                BasicGridView.Items.Add(sender);
            }
            SaveTo();
            if (PinnedView.Items.Count > 0)
            {
                ImageOptionalBlur.Visibility = Visibility.Visible;
                PinnedRow.Height = new GridLength(CalculatedPinHeight());
                BasicGridView.Margin = new Thickness(0, 45 + CalculatedPinHeight(), 0, 0);
            }
            else
            {
                ImageOptionalBlur.Visibility = Visibility.Collapsed;
                PinnedRow.Height = new GridLength();
                BasicGridView.Margin = new Thickness(0, 45, 0, 0);
            }
        }
        
        public int CalculatedPinHeight()
        {
            int calculatedInitial = (int)(Math.Ceiling((500 * PinnedView.Items.Count + PinnedView.Items.Count * 2 + 6) / (CompleteGrid.ActualWidth)) * 302 + 60);
            int convertedHeight = (int)CompleteGrid.ActualHeight;
            if (calculatedInitial <= convertedHeight - 360 && !(convertedHeight - 360 < 360))
            {
                return calculatedInitial;
            }
            else
            {
                return 360;
            }
        }
        public void Pin_Click(object sender, RoutedEventArgs e)
        {
            if((sender as MenuFlyoutItem).Text == AppStrings.widgetPin)
            {
                BasicGridView.Items.Remove((sender as MenuFlyoutItem).Tag as Frame);
                PinnedView.Items.Add((sender as MenuFlyoutItem).Tag as Frame);
            }
            else
            {
                PinnedView.Items.Remove((sender as MenuFlyoutItem).Tag as Frame);
                BasicGridView.Items.Add((sender as MenuFlyoutItem).Tag as Frame);
            }
            SaveTo();
            if (PinnedView.Items.Count > 0)
            {
                ImageOptionalBlur.Visibility = Visibility.Visible;
                PinnedRow.Height = new GridLength(CalculatedPinHeight());
                BasicGridView.Margin = new Thickness(0, 45 + CalculatedPinHeight(), 0, 0);
                PinScrollView.Height = PinnedRow.Height.Value - 60;
            }
            else
            {
                ImageOptionalBlur.Visibility = Visibility.Collapsed;
                PinnedRow.Height = new GridLength(0);
                BasicGridView.Margin = new Thickness(0, 45, 0, 0);
                PinScrollView.Height = 0;
            }
        }
        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            Frame senderWidget = (sender as FrameworkElement).Tag as Frame;
            try
            {
                File.Delete("Config/WidgetNotes/" + senderWidget.Tag.ToString() + ".crlh");
            }
            catch { }
            (senderWidget.Parent as GridView).Items.Remove(senderWidget);
            Thread thread = new(SaveTo);
            thread.Start();
        }
        private void MenuFlyoutEdit_Click(object sender, RoutedEventArgs e)
        {
            Frame senderWidget = (sender as FrameworkElement).Tag as Frame;
            Type type1 = Type.GetType(senderWidget.Content + "WidgetSettings");
            senderWidget.Navigate(type1);
        }
        private void BasicGridView_DragItemsCompleted(ListViewBase sender, DragItemsCompletedEventArgs args)
        {
            Thread thread = new(SaveTo);
            thread.Start();
        }
        private async void UpdateUI()
        {
            string WidgetOrder = "";
            try
            {
                WidgetOrder = File.ReadAllText("Config/WidgetOrder.crlh");
            }
            catch
            {
                Directory.CreateDirectory("Config");
                File.WriteAllText("Config/WidgetOrder.crlh", "");
            }
            WidgetAPI.AddWidgets(BasicGridView, WidgetOrder);
            try
            {
                WidgetOrder = File.ReadAllText("Config/PinnedOrder.crlh");
            }
            catch
            {
                WidgetOrder = "";
                Directory.CreateDirectory("Config");
                File.WriteAllText("Config/PinnedOrder.crlh", "");
            }
            WidgetAPI.AddWidgets(PinnedView, WidgetOrder);
            if (!Globals.HideAddButton)
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    Frame grid = new()
                    {
                        Height = 300,
                        Width = 500,
                        CornerRadius = new CornerRadius(8),
                        Background = (AcrylicBrush)Application.Current.Resources["CustomAcrylicInAppLuminosity"],
                    };
                    grid.Loaded += Workarounds.BugcheckAcrylic;
                    Button button = new()
                    {
                        Name = "AddWidget",
                        Content = "+",
                        FontFamily = new FontFamily("Segoe UI Variable Display Light"),
                        Height = 300,
                        Width = 500,
                        FontSize = 100,
                        Margin = new Thickness(0),
                        CornerRadius = new CornerRadius(8),
                        BorderThickness = new Thickness(0),
                        Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)),
                        AllowDrop = false,
                    };
                    addBtn = button;
                    grid.Content = button;
                    button.Click += Button_Click;
                    BasicGridView.Items.Add(grid);
                });
            }
            if (!Globals.IgnoreUpdates)
            {
                try
                {
                    await MaintenanceAPI.CheckUpdate();
                    if (Globals.IsUpdateAvailable && !Globals.isConfidential)
                    {
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            Frame frame = new()
                            {
                                Name = "UpdateWidget1",
                                CornerRadius = new CornerRadius(8),
                            };
                            frame.RightTapped += Frame_RightTapped;
                            BasicGridView.Items.Insert(0, frame);
                            frame.Navigate(typeof(UpdateWarning));
                        });
                    }
                }
                catch { }
            }
        }
        private void Frame_IsEnabledChanged2(object sender, DependencyPropertyChangedEventArgs _1)
        {
            var frame2 = (Frame)sender;
            if(frame2 != null && frame2.IsEnabled)
            {
                Thread thread = new(SaveTo);
                thread.Start();
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                bool lockReorder = !(Globals.IsAdmin || Globals.LockLayout);
                BasicGridView.CanReorderItems = lockReorder;
                BasicGridView.CanDragItems = lockReorder;
                PinnedView.CanReorderItems = lockReorder;
                PinnedView.CanDragItems = lockReorder;
            });
        }

        private void CompleteGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (PinnedView.Items.Count > 0)
            {
                ImageOptionalBlur.Visibility = Visibility.Visible;
                PinnedRow.Height = new GridLength(CalculatedPinHeight());
                BasicGridView.Margin = new Thickness(0, 45 + CalculatedPinHeight(), 0, 0);
                PinScrollView.Height = PinnedRow.Height.Value + 20;
            }
            else
            {
                ImageOptionalBlur.Visibility = Visibility.Collapsed;
                PinnedRow.Height = new GridLength(0);
                BasicGridView.Margin = new Thickness(0, 45, 0, 0);
                PinScrollView.Height = 0;
            }
        }
        public void ForceBugcheck()
        {
            Workarounds.ForceBugcheckFrames(BasicGridView);
            Workarounds.ForceBugcheckFrames(PinnedView);
        }
        public void KillThis()
        {
            this.NavigationCacheMode = NavigationCacheMode.Disabled;
        }
    }
}
