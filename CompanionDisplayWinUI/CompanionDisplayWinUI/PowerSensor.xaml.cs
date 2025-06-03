using CompanionDisplayWinUI.ClassImplementations;
using LibreHardwareMonitor.Hardware;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Threading;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PowerSensor : Page
    {
        public PowerSensor()
        {
            this.InitializeComponent();
        }
        private bool FTU = true;
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (FTU)
            {
                var parent = this.Parent as Frame;
                if (parent != null)
                {
                    FTU = !FTU;
                    SensorName.Text = parent.Name;
                    sensor1 = parent.Tag as ISensor;
                }
            }
            HardwareSensorsFunction.UpdateSensorValue += UpdateUI;
            Thread thread = new(UpdateUI);
            thread.Start();
        }
        ISensor sensor1;
        public double LoadValue, LoadMin, LoadMax;

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            HardwareSensorsFunction.UpdateSensorValue -= UpdateUI;
        }
        private double LastValue = -1;
        private void UpdateUI()
        {
            Sensors.UpdateSensorValue(sensor1, LastValue, LoadPercent, null, AppStrings.sensorsPower, DispatcherQueue, false);
        }
    }
}
