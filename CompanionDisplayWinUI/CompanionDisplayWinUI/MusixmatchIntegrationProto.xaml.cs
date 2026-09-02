using CompanionDisplayWinUI.API;
using CompanionDisplayWinUI.AppDesign.ArduinoElements;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 

    public sealed partial class MusixmatchIntegrationProto : Page
    {
        public MusixmatchIntegrationProto()
        {
            this.InitializeComponent();
        }

        private ArduinoAPI arduinoFunctionality;

        private void Button_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            arduinoFunctionality.ConnectAndStream();
        }

        private void Content_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            ContentAR.Navigate(typeof(ArduinoInterfaceHomePage));
            arduinoFunctionality = new ArduinoAPI(ContentAR, "COM9");
        }
    }
}
