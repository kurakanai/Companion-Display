using System.IO;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using CompanionDisplayWinUI.API;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WidgetPhotoWidgetSettings : Page
    {
        public WidgetPhotoWidgetSettings()
        {
            this.InitializeComponent();
            try
            {
                string SF = File.ReadAllText(Globals.PhotoConfigFile);
                SmartFlipToggle.IsOn = bool.Parse(SF);
            }
            catch
            {

            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var frame = this.Parent as Frame;
            frame.Navigate(typeof(WidgetPhoto));
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Globals.SmartFlipToggle = SmartFlipToggle.IsOn;
            var parent = this.Parent as Frame;
            parent.Tag = DirectoryTextBox.Text;
            var frame = this.Parent as Frame;
            frame.IsEnabled = false;
            frame.IsEnabled = true;
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(Globals.PhotoConfigFile));
            File.WriteAllText(Globals.PhotoConfigFile, SmartFlipToggle.IsOn.ToString());
            frame.Navigate(typeof(WidgetPhoto));
            
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            string btntag = FileAPI.OpenFileDialog(false)[0];
            DirectoryTextBox.Text = btntag;
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            string btntag2 = FileAPI.OpenFolder();
            DirectoryTextBox.Text = btntag2;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var parent = this.Parent as Frame;
                DirectoryTextBox.Text = parent.Tag.ToString();
                SmartFlipToggle.IsOn = Globals.SmartFlipToggle;
            }
            catch
            {

            }
        }

        private void SmartFlipToggle_Toggled(object sender, RoutedEventArgs e)
        {
            Globals.SmartFlipToggle = SmartFlipToggle.IsOn;
        }
    }
}
