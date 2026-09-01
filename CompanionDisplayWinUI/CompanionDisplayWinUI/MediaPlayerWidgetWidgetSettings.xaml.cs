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
    public sealed partial class MediaPlayerWidgetWidgetSettings : Page
    {
        public MediaPlayerWidgetWidgetSettings()
        {
            this.InitializeComponent();
            CID.Text = Globals._clientId;
            CS.Text = Globals._secretId;
            CID2.Text = Globals._clientId2;
            CS2.Text = Globals._secretId2;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var frame = this.Parent as Frame;
            frame.GoBack();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(Globals.MediaConfigFile));
            File.Delete(Globals.MediaConfigFile);
            File.AppendAllText(Globals.MediaConfigFile, CID.Text + "\n");
            File.AppendAllText(Globals.MediaConfigFile, CS.Text + "\n");
            File.AppendAllText(Globals.MediaConfigFile, CID2.Text + "\n");
            File.AppendAllText(Globals.MediaConfigFile, CS2.Text + "\n");
            Globals._clientId = CID.Text;
            Globals._secretId = CS.Text;
            Globals._clientId2 = CID2.Text;
            Globals._secretId2 = CS2.Text;
            Globals.RefreshToken = "";
            Globals.RefreshToken2 = "";
            if (File.Exists(Globals.RefreshTokenPath))
            {
                File.Delete(Globals.RefreshTokenPath);
            }
            if (File.Exists(Globals.RefreshToken2Path))
            {
                File.Delete(Globals.RefreshToken2Path);
            }
            var frame = this.Parent as Frame;
            frame.GoBack();
        }
    }
}
