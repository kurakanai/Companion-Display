using CompanionDisplayWinUI.API;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using System.Threading;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WidgetPhoneIndividual : Page
    {
        double Brightness1;
        string ID;
        bool SliderInt = false;
        public WidgetPhoneIndividual()
        {
            this.InitializeComponent();
        }
        public bool CleanUp = false;
        private double LastLvl = -1, LastBrightness = -1;
        private string LastName = "-";
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Thread thread2 = new(() => CommandAPI.PerformCMDCommand("runtimes\\adb.exe - s " + ID + " reboot"));
            thread2.Start();
        }
        private void UpdateUI()
        {
            string DevName = ADBAPI.getDeviceName(ID);
            int DevBattery = ADBAPI.getDeviceBatteryLevel(ID);
            double DevBrightness = ADBAPI.getDeviceBrightness(ID);
            string DevBatteryIcon = BatteryAPI.GetBatteryIcon(DevBattery);
            if (DevBattery != LastLvl)
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    BatteryIcon.Text = DevBatteryIcon;
                    Device1BatteryInt.Text = DevBattery.ToString() + "%";
                });
                LastLvl = DevBattery;
            }
            if(DevBrightness != LastBrightness)
            {
                LastBrightness = DevBrightness;
                DispatcherQueue.TryEnqueue(() =>
                {
                    Device1Brightness.Value = DevBrightness;
                });
            }            
            if(DevName != LastName)
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    Device1Name.Text = DevName;
                });
                LastName = DevName;
            }
            if (!CleanUp)
            {
                Thread.Sleep(1000);
                Thread thread0 = new(UpdateUI);
                thread0.Start();
            }
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Thread thread2 = new(() => CommandAPI.PerformCMDCommand("runtimes\\adb.exe -s " + ID + " shell input keyevent 26"));
            thread2.Start();
        }
        private void Device1Brightness_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Brightness1 = Device1Brightness.Value;
            Thread thread = new(ChangeBrightness1);
            thread.Start();
        }
        private void ChangeBrightness1()
        {
        start:
            if (!SliderInt)
            {
                SliderInt = !SliderInt;
                CommandAPI.PerformCMDCommand("runtimes\\adb.exe -s " + ID + " shell settings put system screen_brightness " + Brightness1);
                SliderInt = !SliderInt;
            }
            else
            {
                goto start;
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var parent = this.Parent as Frame;
            ID = parent.Name;
            Thread thread0 = new(UpdateUI);
            thread0.Start();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            CleanUp = true;
        }
    }
}
