using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.IO;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TimeWidgetWidgetSettings : Page
    {
        public TimeWidgetWidgetSettings()
        {
            this.InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var frame = this.Parent as Frame;
            frame.GoBack();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(Globals.TimeConfigFile));
            File.Delete(Globals.TimeConfigFile);
            File.AppendAllText(Globals.TimeConfigFile, TimeColorPicker.Color.R + "\n");
            File.AppendAllText(Globals.TimeConfigFile, TimeColorPicker.Color.G + "\n");
            File.AppendAllText(Globals.TimeConfigFile, TimeColorPicker.Color.B + "\n");
            File.AppendAllText(Globals.TimeConfigFile, DateColorPicker.Color.R + "\n");
            File.AppendAllText(Globals.TimeConfigFile, DateColorPicker.Color.G + "\n");
            File.AppendAllText(Globals.TimeConfigFile, DateColorPicker.Color.B + "\n");
            var frame = this.Parent as Frame;
            frame.GoBack();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            File.Delete(Globals.TimeConfigFile);
            var frame = this.Parent as Frame;
            frame.GoBack();
        }
    }
}
