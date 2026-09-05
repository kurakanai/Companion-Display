using CompanionDisplayWinUI.API;
using CompanionDisplayWinUI.ClassImplementations;
using CompanionDisplayWinUI.ClassImplementations.SharedPages;
using LibreHardwareMonitor.Hardware;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
using static System.Net.WebRequestMethods;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI.Pages.Widgets.Sensors.SharedAssets;

public sealed partial class SharedSensorLayout : UserControl
{
    public SharedSensorLayout(ISensor sensor)
    {
        InitializeComponent();
        this.sensor = sensor;
        switch (sensor.SensorType)
        {
            case SensorType.Load:
                sensorSuffix = "%";
                break;
            case SensorType.Clock:
                sensorSuffix = "MHz";
                break;
            case SensorType.Power:
                sensorSuffix = "W";
                break;
            case SensorType.SmallData:
                sensorSuffix = "MB";
                break;
            case SensorType.Factor:
                sensorSuffix = "x";
                break;
            case SensorType.Fan:
                sensorSuffix = "RPM";
                break;
            case SensorType.Temperature:
                sensorSuffix = "ªC";
                break;
            case SensorType.Voltage:
                sensorSuffix = "V";
                break;
        }
        SensorTypeTextBox.Text = sensor.SensorType.ToString();
        SensorNameTextBox.Text = sensor.Name;
        TempProgress.Visibility = (sensorSuffix == "%" || sensorSuffix == "ªC") ? Visibility.Visible : Visibility.Collapsed;
        needsExtraPrecision = sensorSuffix == "V";
    }
    private string sensorSuffix;
    ISensor sensor;
    public bool needsExtraPrecision;
    public double lastValue;
    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        HardwareMonitorAPI.UpdateSensorValueEvent += UpdateUI;
        Thread thread = new(UpdateUI);
        thread.Start();
    }
    public void Page_Unloaded(object _1, RoutedEventArgs _2)
    {
        HardwareMonitorAPI.UpdateSensorValueEvent -= UpdateUI;
    }
    private void UpdateUI()
    {
        HardwareMonitorAPI.UpdateSensorValue(sensor, lastValue, Load, TempProgress, sensorSuffix, DispatcherQueue, needsExtraPrecision);
    }

}
