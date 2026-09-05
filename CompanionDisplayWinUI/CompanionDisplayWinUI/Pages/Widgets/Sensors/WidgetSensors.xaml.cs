using CompanionDisplayWinUI.API;
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
    public sealed partial class WidgetSensors : Page
    {
        public WidgetSensors()
        {
            this.InitializeComponent();
        }
        private bool FTU = true;
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (FTU)
            {
                Thread thread = new(UpdateUI);
                thread.Start();
                FTU = false;
            }
        }
        private void UpdateUI()
        {
            HardwareMonitorAPI hardwareSensorsFunction = new();
            hardwareSensorsFunction.Init();
        start:
            if (hardwareSensorsFunction.computer.Hardware.Count != 0)
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    NoDevices.Visibility = Visibility.Collapsed;
                    foreach (IHardware hardware in hardwareSensorsFunction.computer.Hardware)
                    {
                        if(hardware.Sensors.Length > 0 || (hardware.SubHardware.Length > 0 && hardware.SubHardware[0].Sensors.Length > 0))
                        {
                            Frame frame = new()
                            {
                                Tag = hardware,
                                Name = hardware.Name
                            };
                            FlipViewHW.Items.Add(frame);
                            frame.Navigate(typeof(IndividualComponent));
                        }
                    }
                });
            }
            else
            {
                Thread.Sleep(1000);
                goto start;
            }
            DispatcherQueue.TryEnqueue(() =>
            {
                Globals.CurrentHW = (FlipViewHW.SelectedItem as Frame).Tag as IHardware;
            });
        }

        private void FlipViewHW_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Globals.CurrentHW = (FlipViewHW.SelectedItem as Frame).Tag as IHardware;
        }
    }
}
