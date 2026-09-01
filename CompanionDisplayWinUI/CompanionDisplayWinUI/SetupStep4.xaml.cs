using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Animation;
using CompanionDisplayWinUI.ClassImplementations;
using CompanionDisplayWinUI.API;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SetupStep4 : Page
    {
        public SetupStep4()
        {
            this.InitializeComponent();
        }

        private void Button_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var frame = this.Parent as Frame;
            frame.GoBack();
        }

        private void Button_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            Globals.triggerSetup = false;
            ConfigAPI.Save_Settings();
            var frame = this.Parent as Frame;
            CommonlyAccessedInstances.nvSample.IsPaneVisible = true;
            CommonlyAccessedInstances.nvSample.SelectedItem = CommonlyAccessedInstances.nvSample.MenuItems[1];
            frame.Navigate(typeof(BlankPage1), null, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
        }
    }
}
