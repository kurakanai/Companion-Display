using CompanionDisplayWinUI.API;
using CompanionDisplayWinUI.ClassImplementations;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System.IO;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BlackBoxRecover : Window
    {
        public BlackBoxRecover()
        {
            InitializeComponent();
            try{
                Error.Text = File.ReadAllText(Globals.BlackBoxFile);
            }
            catch {
                Error.Text = "Unable to fetch crash log.";
            }
        }

        private void Button_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                if (File.Exists(Globals.BlackBoxFile))
                {
                    File.Delete(Globals.BlackBoxFile);
                }
                App.StartApp();
                this.Close();
            }
            catch{
                ((Button)sender).Content = "Launch Failed!";
            }
        }

        private void Button_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
             try{
                ((Button)sender).Content = "Attempting Online Update...";
                MainStackPanel.IsHitTestVisible = false;
                if (File.Exists(Globals.BlackBoxFile))
                {
                    File.Delete(Globals.BlackBoxFile);
                }
                MaintenanceAPI.PerformUpdate(true);
            }
            catch {
                ((Button)sender).Content = "Online Update Failed!";
                MainStackPanel.IsHitTestVisible = true;
            }
        }

        private void Button_Tapped_2(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                ((Button)sender).Content = "Attempting Local Update...";
                MainStackPanel.IsHitTestVisible = false;
                if (File.Exists(Globals.BlackBoxFile))
                {
                    File.Delete(Globals.BlackBoxFile);
                }
                MaintenanceAPI.PerformUpdate(false);
                MainStackPanel.IsHitTestVisible = true;
                ((Button)sender).Content = "Attempt Local Reinstall";
            }
            catch
            {
                ((Button)sender).Content = "Local Update Failed!";
                MainStackPanel.IsHitTestVisible = true;
            }
        }

        private void Button_Tapped_3(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                ((Button)sender).Content = "Attempting Backup...";
                ((Button)sender).IsEnabled = false;
                BackupAPI.OpenDialog(this.Content.XamlRoot, true);
                MainStackPanel.IsHitTestVisible = true;
                ((Button)sender).Content = "Backup Menu";
            }
            catch
            {
                ((Button)sender).Content = "Backup Failed!";
                MainStackPanel.IsHitTestVisible = true;
            }
            ((Button)sender).IsEnabled = false;
        }

        private void Button_Tapped_4(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                ((Button)sender).Content = "Resetting...";
                ((Button)sender).IsEnabled = false;
                BackupAPI.EraseConfig();
                MainStackPanel.IsHitTestVisible = true;
                ((Button)sender).Content = "Reset Config";
            }
            catch
            {
                ((Button)sender).Content = "Reset Failed";
                MainStackPanel.IsHitTestVisible = true;
            }
            ((Button)sender).IsEnabled = true;
        }
    }
}
