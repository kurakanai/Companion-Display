using CompanionDisplayWinUI.API;
using CoreAudio;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WidgetAudioControl : Page
    {
        public WidgetAudioControl()
        {
            this.InitializeComponent();
            MMDevice[] endpointCollection = AudioCoreAPI.getAllEndpoints;
            for(int i  = 0; i < endpointCollection.Length; i++)
            {
                MMDevice endpoint = endpointCollection[i];
                MenuFlyoutItem item = new()
                {
                    Text = endpoint.FriendlyName,
                    Tag = endpoint
                };
                item.Click += MenuFlyoutItem_Click;
                ListOfDevices.Items.Add(item);
            }
            CurrentDevice.Content = AudioCoreAPI.mmDevices.FriendlyName;
            DeviceView.Tag = AudioCoreAPI.mmDevices;
            DeviceView.Navigate(typeof(AudioDevice));
        }

        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem selecteditem = sender as MenuFlyoutItem;
            CurrentDevice.Content = selecteditem.Text;
            DeviceView.Tag = selecteditem.Tag;
            DeviceView.Navigate(typeof(AudioDevice));
        }
    }
}
