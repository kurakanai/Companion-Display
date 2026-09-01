using CompanionDisplayWinUI.API;
using LibreHardwareMonitor.Hardware;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Threading;
namespace CompanionDisplayWinUI.ClassImplementations.SharedPages
{
    public class SharedSensorPage : Page
    {
        public TextBlock sensorText, sensorName;
        public ProgressRing sensorRing;
        public bool needsExtraPrecision;
        public double lastValue;
        public string sensorSuffix;
        ISensor sensor1;
        public bool FTU = true;
        public void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (FTU)
            {
                var parent = this.Parent as Frame;
                if (parent != null)
                {
                    FTU = false;
                    sensorName.Text = parent.Name;
                    sensor1 = parent.Tag as ISensor;
                }
            }
            HardwareMonitorAPI.UpdateSensorValueEvent += UpdateUI;
            Thread thread = new(UpdateUI);
            thread.Start();
        }

        public void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            HardwareMonitorAPI.UpdateSensorValueEvent -= UpdateUI;
        }
        private void UpdateUI()
        {
            HardwareMonitorAPI.UpdateSensorValue(sensor1, lastValue, sensorText, sensorRing, sensorSuffix, DispatcherQueue, needsExtraPrecision);
        }
    }
}
