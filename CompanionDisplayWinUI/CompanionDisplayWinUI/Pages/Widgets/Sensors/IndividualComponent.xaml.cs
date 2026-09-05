using CompanionDisplayWinUI.ClassImplementations.SharedPages;
using CompanionDisplayWinUI.Pages.Widgets.Sensors.SharedAssets;
using LibreHardwareMonitor.Hardware;
using LibreHardwareMonitor.Hardware.Motherboard;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class IndividualComponent : Page
    {
        public IndividualComponent()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }
        public bool LoadFinished = false;
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (!LoadFinished)
            {
                try
                {
                    var parent = this.Parent as Frame;
                    ComponentType.Text = parent.Name;
                    Hardware hardware = parent.Tag as Hardware;
                    try
                    {
                        Motherboard motherboard = parent.Tag as Motherboard;
                        foreach (IHardware subhardware in motherboard.SubHardware)
                        {
                            foreach (ISensor sensor in subhardware.Sensors)
                            {
                                if (sensor.Value != null)
                                {
                                    Frame frame = new()
                                    {
                                        Tag = sensor,
                                        Name = sensor.Name
                                    };
                                    AddSensors(sensor, frame);
                                }
                            }
                        }
                        NoSensorsWarning.Visibility = (Visibility)Convert.ToByte(!(ComponentSensorStack.Children.Count == 1));
                    }
                    catch { }
                    foreach (ISensor sensor in hardware.Sensors)
                    {
                        Frame frame = new()
                        {
                            Tag = sensor,
                            Name = sensor.Name
                        };
                        AddSensors(sensor, frame);
                    }
                    NoSensorsWarning.Visibility = (Visibility)Convert.ToByte(!(ComponentSensorStack.Children.Count == 1));
                }
                catch { }
                LoadFinished = true;
            }
        }
        private void AddSensors(ISensor sensor, Frame frame)
        {
            SharedSensorLayout sharedSensorLayout = new SharedSensorLayout(sensor);
            ComponentSensorStack.Children.Add(sharedSensorLayout);
        }
    }
}
