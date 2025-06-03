using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using CoreAudio;
using CompanionDisplayWinUI.ClassImplementations;

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
        }
        private bool FTU = true;
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (FTU)
            {
                var device = AudioManager.DevEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
                foreach (var endpoint in AudioManager.DevEnum.EnumerateAudioEndPoints(EDataFlow.eRender, DEVICE_STATE.DEVICE_STATE_ACTIVE))
                {
                    MenuFlyoutItem item = new()
                    {
                        Text = endpoint.FriendlyName,
                        Tag = endpoint
                    };
                    item.Click += MenuFlyoutItem_Click;
                    ListOfDevices.Items.Add(item);
                }
                CurrentDevice.Content = device.FriendlyName;
                DeviceView.Tag = device;
                DeviceView.Navigate(typeof(AudioDevice));
                FTU = !FTU;
            }
        }

        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            var selecteditem = sender as MenuFlyoutItem;
            CurrentDevice.Content = selecteditem.Text;
            DeviceView.Tag = selecteditem.Tag;
            DeviceView.Navigate(typeof(AudioDevice));
        }
    }
}
