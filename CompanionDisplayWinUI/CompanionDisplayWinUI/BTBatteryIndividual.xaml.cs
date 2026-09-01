using CompanionDisplayWinUI.API;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Threading;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BTBatteryIndividual : Page
    {
        public BTBatteryIndividual()
        {
            this.InitializeComponent();
        }
        private new string Name;
        private bool FTU = true;
        private void UpdateUI()
        {
            string script = @"Set-ExecutionPolicy -Scope CurrentUser -ExecutionPolicy Bypass -Force;
$BTDeviceFriendlyName = """ + Name + @"""
$Shell = New-Object -ComObject ""WScript.Shell""
$BTHDevices = Get-PnpDevice -FriendlyName ""*$($BTDeviceFriendlyName)*""
$BatteryLevels = foreach ($Device in $BTHDevices) 
{
        $BatteryProperty = Get-PnpDeviceProperty -InstanceId $Device.InstanceId -KeyName '{104EA319-6EE2-4701-BD47-8DDBF425BBE5} 2' |
            Where-Object { $_.Type -ne 'Empty' } |
            Select-Object -ExpandProperty Data

        if ($BatteryProperty) {
            $BatteryProperty
        }
    }
$Message = ""$BatteryLevels%""
$Message
";
            try
            {
                using PowerShell powerShell = PowerShell.Create();
                powerShell.AddScript(script);
                Collection<PSObject> results = powerShell.Invoke();
                foreach (var result in results)
                {
                    string battIcon = BatteryAPI.GetBatteryIcon(int.Parse(result.ToString()[..^1]));
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        Percentage.Text = result.ToString();
                        BattIcon.Text = battIcon;
                    });
                }
                Thread.Sleep(10000);
                Thread thread = new(UpdateUI);
                thread.Start();
            }
            catch {
                DispatcherQueue.TryEnqueue(() =>
                {
                    ((this.Parent as Frame).Parent as StackPanel).Children.Remove((this.Parent) as Frame);
                });
            }
        }
        private void PercentageChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            ReadBTLEBatt(args.CharacteristicValue);
        }
        private void ReadBTLEBatt(IBuffer Value)
        {
            var reader = DataReader.FromBuffer(Value);
            string percentage = reader.ReadByte().ToString();
            int DevBattery = int.Parse(percentage);
            string battIcon = BatteryAPI.GetBatteryIcon(int.Parse(percentage));
            DispatcherQueue.TryEnqueue(() =>
            {
                BattIcon.Text = battIcon;
                Percentage.Text = percentage + "%";
            });
        }
        BluetoothLEDevice device;
        GattCharacteristic batteryLevelCharacteristic;
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (FTU)
                {
                    Name = (this.Parent as Frame).Name;
                    DevName.Text = Name;
                    if ((this.Parent as Frame).Tag as BluetoothLEDevice != null)
                    {
                        device = (this.Parent as Frame).Tag as BluetoothLEDevice;
                        DevName.Text = device.Name;
                        GattDeviceServicesResult batteryService = await device.GetGattServicesForUuidAsync(GattServiceUuids.Battery);
                        GattCharacteristicsResult service = await batteryService.Services.FirstOrDefault().GetCharacteristicsForUuidAsync(GattCharacteristicUuids.BatteryLevel);
                        batteryLevelCharacteristic = service.Characteristics.FirstOrDefault();
                        var result = await batteryLevelCharacteristic.ReadValueAsync();
                        batteryLevelCharacteristic.ValueChanged -= PercentageChanged;
                        batteryLevelCharacteristic.ValueChanged += PercentageChanged;
                        if (result.Status == GattCommunicationStatus.Success)
                        {
                            ReadBTLEBatt(result.Value);
                        }
                        else
                        {
                            ((this.Parent as Frame).Parent as StackPanel).Children.Remove((this.Parent) as Frame);
                        }
                    }
                    else
                    {
                        Thread thread = new(UpdateUI);
                        thread.Start();
                    }
                    FTU = !FTU;
                }
                else
                {
                    batteryLevelCharacteristic.ValueChanged -= PercentageChanged;
                }
            }
            catch { }
        }
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                batteryLevelCharacteristic.ValueChanged -= PercentageChanged;
            }
            catch { }
        }
    }
}
