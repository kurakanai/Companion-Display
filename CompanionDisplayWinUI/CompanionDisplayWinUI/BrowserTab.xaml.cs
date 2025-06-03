using System;
using System.IO;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using Windows.System;
using CompanionDisplayWinUI.ClassImplementations;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BrowserTab : Page
    {
        public BrowserTab()
        {
            this.InitializeComponent();
            BrowserTabs.CanDrag = !Globals.IsAdmin;
            BrowserTabs.CanDragTabs = !Globals.IsAdmin;
            BrowserTabs.CanReorderTabs = !Globals.IsAdmin;
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }
        private void MenuFlyoutItem5_Click(object sender, RoutedEventArgs e)
        {
            foreach (UIElement item in Launchpad.Items.Cast<UIElement>())
            {
                if (item is Grid)
                {
                    Grid grid = item as Grid;
                    var childControl = (Microsoft.UI.Xaml.Controls.Button)grid.Children[0];
                    childControl.IsHitTestVisible = false;
                }
            }
            Button button = BrowserClass.CreateLaunchPadButton("+", new Microsoft.UI.Xaml.Media.FontFamily("Segoe UI Variable Display Light"), "AddWidget");
            Button button2 = BrowserClass.CreateLaunchPadButton("\ue73e", new Microsoft.UI.Xaml.Media.FontFamily("Segoe Fluent Icons"), "Finish");
            NoItem.Visibility = Visibility.Collapsed;
            button.Click += Button_Click_1a;
            button2.Click += Button_Click0;
            Launchpad.Items.Add(button);
            Launchpad.Items.Add(button2);
        }
        private void MenuFlyoutItem5a_Click(object sender, RoutedEventArgs e)
        {
            Launchpad.Items.Remove((sender as MenuFlyoutItem).Tag as Grid);
            Launchpad_LayoutUpdated(Launchpad, null);
        }
        private async void Button_Click_1a(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new()
            {
                XamlRoot = this.XamlRoot,
                Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                Title = AppStrings.browserLaunchpadAdd,
                PrimaryButtonText = AppStrings.browserLaunchpadFlyoutAdd,
                CloseButtonText = AppStrings.cancelString,
                DefaultButton = ContentDialogButton.Primary
            };
            TextBox textBox = new()
            {
                Margin = new Thickness(0),
                Height = 30,
                PlaceholderText = AppStrings.browserLaunchpadURLTemplate
            };
            dialog.Content = textBox;
            dialog.Tag = (sender as Button).Tag;
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                try
                {
                    string URL = (dialog.Content as TextBox).Text;
                    Uri myUri = new("https://" + URL);
                    Image image = BrowserClass.GetWebsiteIcon(myUri);
                    Grid grid0 = new();
                    Grid grid1 = new()
                    {
                        CornerRadius = new CornerRadius(8),
                        Width = 150,
                        Height = 150
                    };
                    grid1.Children.Add(image);
                    Button button = BrowserClass.CreateLaunchPadButton(grid1, new Microsoft.UI.Xaml.Media.FontFamily("Segoe UI Variable Display Light"), "");
                    button.Tag = myUri;
                    grid0.RightTapped += IndividualItemRC;
                    grid0.Children.Add(button);
                    button.Click += NewTabLaunchpad;
                    Launchpad.Items.Add(grid0);
                    Launchpad_LayoutUpdated(Launchpad, null);
                }
                catch { }
            }
        }

        private void IndividualItemRC(object sender, RightTappedRoutedEventArgs e)
        {
            FrameworkElement senderElement = sender as FrameworkElement;
            MenuFlyout myFlyout = new();
            MenuFlyoutItem fifthItem = new() { Text = AppStrings.browserLaunchpadRemove, Name = senderElement.Name + "Remove", Tag = sender };
            fifthItem.Click += MenuFlyoutItem5a_Click;
            if (Launchpad.FindName("AddWidget") == null)
            {
                myFlyout.Items.Add(fifthItem);
            }
            myFlyout.ShowAt(senderElement, new Windows.Foundation.Point(0, 0));
        }

        private void NewTabLaunchpad(object sender, RoutedEventArgs e)
        {
            NewTabLaunchpad((sender as Button).Tag as Uri);
        }
        private void NewTabLaunchpad(Uri uri)
        {
            var frame = this.Parent as Frame;
            var navviewparent = frame.Parent as NavigationView;
            TabViewItem tab = CreateNewTVI(AppStrings.browserNewTab, AppStrings.browserNewTab, uri, navviewparent, true);
            BrowserTabs.TabItems.Remove(BrowserTabs.SelectedItem);
            BrowserTabs.TabItems.Add(tab);
            BrowserTabs.SelectedItem = tab;
        }
        private void GridView_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            FrameworkElement senderElement = sender as FrameworkElement;
            MenuFlyout myFlyout = new();
            MenuFlyoutItem fifthItem = new()
            {
                Text = AppStrings.browserLaunchpadEdit,
                Tag = sender
            };
            fifthItem.Click += MenuFlyoutItem5_Click;
            if (Launchpad.FindName("AddWidget") == null)
            {
                myFlyout.Items.Add(fifthItem);
            }
            myFlyout.ShowAt(senderElement, new Windows.Foundation.Point(0, 0));
        }
        private void Button_Click0(object sender, RoutedEventArgs e)
        {
            var childControl = (Microsoft.UI.Xaml.Controls.Button)Launchpad.FindName("AddWidget");
            var childControl2 = (Microsoft.UI.Xaml.Controls.Button)Launchpad.FindName("Finish");
            Launchpad.Items.Remove(childControl);
            Launchpad.Items.Remove(childControl2);
            foreach (UIElement item in Launchpad.Items.Cast<UIElement>())
            {
                if (item is Grid)
                {
                    try
                    {
                        Grid grid = item as Grid;
                        var childControl3 = (Microsoft.UI.Xaml.Controls.Button)grid.Children[0];
                        childControl3.IsHitTestVisible = true;
                    }
                    catch { }
                }
            }
            NoItem.Visibility = (Visibility)Convert.ToByte(!(Launchpad.Items.Count == 0));
        }
        private static TabViewItem CreateNewTVI(string header, string dataContext, System.Uri uri, NavigationView navigationView, bool createContent)
        {
            var newTab = new TabViewItem()
            {
                IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource()
                {
                    Symbol = Symbol.Placeholder
                },
                Header = header,
            };
            if (createContent)
            {
                newTab.Content = new BlankPage2(uri, navigationView)
                {
                    DataContext = dataContext
                };
            }
            return newTab;
        }
        private void AddressBar_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                NewTabLaunchpad(BrowserClass.ParseLink(AddressBar.Text));
            }
        }
        private void Tabs_AddTabButtonClick(TabView sender, object args)
        {
            var frame = this.Parent as Frame;
            var navviewparent = frame.Parent as NavigationView;
            TabViewItem tab;
            if(Globals.NewTabBehavior == 0)
            {
                tab = CreateNewTVI(AppStrings.browserLaunchpadTab, AppStrings.browserLaunchpadTab, null, null, false);
            }
            else
            {
                tab = CreateNewTVI(AppStrings.browserNewTab, AppStrings.browserNewTab, null, navviewparent, true);
            }
            sender.TabItems.Add(tab);
            sender.SelectedItem = tab;
        }
        private void Tabs_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {
            try
            {
                var tab = args.Tab;
                var tabitself = tab.Content as BlankPage2;
                tabitself.CloseTab();
                if (sender.SelectedItem as BlankPage2 == tabitself)
                {
                    if (sender.SelectedIndex == 1)
                    {
                        try
                        {
                            sender.SelectedIndex++;
                        }
                        catch
                        {
                            sender.SelectedIndex = -1;
                        }
                    }
                    if (sender.SelectedIndex >= 2)
                    {
                        sender.SelectedIndex --;
                    }
                }
            }
            catch
            {
                
            }
            sender.TabItems.Remove(args.Tab);
        }
        public void CloseEverything()
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                foreach (var item in BrowserTabs.TabItems)
                {
                    BrowserTabs.TabItems.Remove(item);
                }
            });
        }

        private void Launchpad_LayoutUpdated(object sender, object e)
        {
            if (!FTU)
            {
                string Apps = "";
                foreach (UIElement item in Launchpad.Items.Cast<UIElement>())
                {
                    if (item is Grid)
                    {
                        Grid grid = item as Grid;
                        var childControl3 = grid.Children[0] as Button;
                        Apps = Apps + childControl3.Tag.ToString() + "\n";
                    }
                }
                File.WriteAllText("Config/LaunchpadBrowserConfig.crlh", Apps);
            }
        }
        bool FTU = true;
        string WidgetOrder;
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (FTU)
            {
                try
                {
                    WidgetOrder = File.ReadAllText("Config/LaunchpadBrowserConfig.crlh");
                }
                catch
                {
                    File.AppendAllText("Config/LaunchpadBrowserConfig.crlh", "");
                }
                DispatcherQueue.TryEnqueue(() =>
                {
                    try
                    {
                        foreach (string line in WidgetOrder.Split('\n'))
                        {
                            if (line.Length != 0)
                            {
                                string fix = line.Replace("\r", "");
                                Grid grid = new();
                                Grid grid1 = new()
                                {
                                    CornerRadius = new CornerRadius(10),
                                    Width = 150,
                                    Height = 150
                                };
                                grid1.Children.Add(BrowserClass.GetWebsiteIcon(new System.Uri(fix)));
                                Button button = BrowserClass.CreateLaunchPadButton(grid1, new Microsoft.UI.Xaml.Media.FontFamily("Segoe UI Variable Display Light"), "");
                                button.Tag = new System.Uri(line);
                                button.Click += NewTabLaunchpad;
                                grid.RightTapped += IndividualItemRC;
                                grid.Children.Add(button);
                                Launchpad.Items.Add(grid);
                            }
                        }
                    }
                    catch
                    {

                    }
                    Launchpad.Visibility = (Visibility)Convert.ToByte(!(Launchpad.Items.Count == 0));
                });
                FTU = !FTU;
                bool isAdmin = Globals.IsAdmin || Globals.LockLayout;
                DispatcherQueue.TryEnqueue(() =>
                {
                    Launchpad.CanReorderItems = isAdmin;
                    Launchpad.CanDragItems = isAdmin;
                });
            }
        }

        private void Launchpad_LayoutUpdated_1(object sender, object e)
        {
            AddressBar.Width = Launchpad.ActualWidth;
        }
    }
}
