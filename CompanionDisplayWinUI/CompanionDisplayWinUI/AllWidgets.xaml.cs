using CompanionDisplayWinUI.API;
using CompanionDisplayWinUI.ClassImplementations;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AllWidgets : Page
    {
        private string targetFile = "Config/WidgetOrder.crlh";
        public AllWidgets()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
            var currentAssembly = this.GetType().GetTypeInfo().Assembly;
            var pageTypeInfo = typeof(Page).GetTypeInfo();
            int i = 1;
            foreach (var item in currentAssembly.DefinedTypes.Where(t => pageTypeInfo.IsAssignableFrom(t)).ToList())
            {
                if (item.Name.Contains("Widget") && item.Name != "AllWidgets" && !item.Name.Contains("WidgetSettings") && !item.Name.Contains("Individual"))
                {
                    Frame frame = new()
                    {
                        Name = "Widget" + i,
                        CornerRadius = new CornerRadius(8),
                        Background = (AcrylicBrush)Application.Current.Resources["CustomAcrylicInAppLuminosity"]
                    };
                    frame.Loaded += Workarounds.BugcheckAcrylic;
                    frame.IsHitTestVisible = false;
                    frame.Navigate(item);
                    try
                    {
                        String grouper = (frame.Content as Page).Tag.ToString();
                        (AllCategories.FindName(grouper + "GridView") as GridView).Items.Add(frame);
                    }
                    catch { }
                }
            }
        }
        private void BasicGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if(!((e.ClickedItem as Frame).Content.ToString().Contains("WidgetStack") && targetFile != "Config/WidgetOrder.crlh"))
            {
                if(PageCacheHandler.homePage != null){
                    PageCacheHandler.homePage.killThis();
                    PageCacheHandler.homePage = null;
                }
                try
                {
                    Globals.IsAllApps = false;
                    var frame = e.ClickedItem as Frame;
                    if (frame.Content.ToString().Contains("NotesWidget") || frame.Content.ToString().Contains("WidgetStack"))
                    {
                        File.AppendAllText(targetFile, frame.Content + "ID" + DateTime.Now.ToString("yyyymmddhhmmssff") + Environment.NewLine);
                    }
                    else
                    {
                        File.AppendAllText(targetFile, frame.Content + Environment.NewLine);
                    }
                    var frame2 = this.Parent as Frame;
                    var navviewparent = frame2.Parent as NavigationView;
                    frame2.GoBack();
                    navviewparent.SelectedItem = (Microsoft.UI.Xaml.Controls.NavigationViewItem)navviewparent.FindName("HomeItem");
                }
                catch
                {
                    Globals.IsAllApps = true;
                }
            }
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            foreach (GridView gridView in AllCategories.Children.Cast<GridView>())
            {
                gridView.Visibility = Visibility.Collapsed;
            }
            (AllCategories.FindName((sender.SelectedItem as NavigationViewItem).Name + "GridView") as GridView).Visibility = Visibility.Visible;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (((this.Parent as Frame).Parent as NavigationView).Tag is not null and not (object)"")
            {
                targetFile = "Config/Stacks/" + ((this.Parent as Frame).Parent as NavigationView).Tag.ToString() + ".crlh";
                ((this.Parent as Frame).Parent as NavigationView).Tag = "";
            }
            else
            {
                targetFile = "Config/WidgetOrder.crlh";
            }
        }
    }
}
