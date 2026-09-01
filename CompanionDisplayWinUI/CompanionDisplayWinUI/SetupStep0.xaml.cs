using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Animation;
using CompanionDisplayWinUI.ClassImplementations;
using CompanionDisplayWinUI.API;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CompanionDisplayWinUI
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class SetupStep0 : Page
	{
		public SetupStep0()
		{
			this.InitializeComponent();
		}
        private void Button_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Globals.triggerSetup = false;
            ConfigAPI.Save_Settings();
            var frame = this.Parent as Frame;
            frame.Navigate(typeof(SetupStep2), null, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
        }

        private void HyperlinkButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Globals.triggerSetup = false;
            ConfigAPI.Save_Settings();
            var frame = this.Parent as Frame;
            CommonlyAccessedInstances.nvSample.IsPaneVisible = true;
            CommonlyAccessedInstances.nvSample.SelectedItem = CommonlyAccessedInstances.nvSample.MenuItems[1];
            frame.Navigate(typeof(BlankPage1), null, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
        }

        private void HyperlinkButton_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            BackupAPI.BackupFinished += HidePane;
            BackupGrid.Visibility = Visibility.Visible;
            BackupAPI.OpenDialog(this.XamlRoot, false);
        }
        private void HidePane()
        {
            BackupAPI.BackupFinished -= HidePane;
            BackupGrid.Visibility = Visibility.Collapsed;
        }
    }
}
