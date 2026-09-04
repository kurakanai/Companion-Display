using CompanionDisplayWinUI.API;
using CompanionDisplayWinUI.ClassImplementations;
using CompanionDisplayWinUI.Objects;
using CoreAudio;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;
using Windows.Devices.Radios;
using Windows.Media.Control;
using Windows.UI.ViewManagement;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TimeWidget : Page
    {
        public TimeWidget()
        {
            InitializeComponent();
            DataContext = Globals.publicTimeViewModel;
        }
        private bool isVisible = false;
        public bool configChanged = true;
        SongObject songObject = MusicAPI.currentSong;
        private int loadCountPerUnload = 0;
        private void SongUpdated()
        {
            songObject = MusicAPI.currentSong;
            DispatcherQueue.TryEnqueue(() =>
            {
                try
                {
                    SongTitle.Text = songObject.title;
                    Album.Source = songObject.albumCover;
                    Album2.Source = Album.Source;
                }
                catch { }
            });
        }
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            if (isVisible && loadCountPerUnload == 1)
            {
                try
                {
                    isVisible = false;
                    radio1.StateChanged -= UpdateWiFi;
                    radio2.StateChanged -= UpdateBT;
                    mDevice.AudioEndpointVolume.OnVolumeNotification -= ChangeVol;
                    MusicAPI.CallInfoUpdate -= SongUpdated;
                    MusicAPI.currentSession.PlaybackInfoChanged -= PlayPauseUpdated;
                }
                catch { }
                loadCountPerUnload = 0;
            }
        }
        public bool IsBluetoothOn()
        {
            return (radio2.State == RadioState.On);
        }
        private Radio radio1, radio2;
        private async void BTToggle_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                if (BTToggle.IsChecked.Value)
                {
                    await radio2.SetStateAsync(RadioState.On);
                }
                else
                {
                    await radio2.SetStateAsync(RadioState.Off);
                }
            }
            catch { }
        }

        private async void WiFiToggle_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                if (WiFiToggle.IsChecked.Value)
                {
                    await radio1.SetStateAsync(RadioState.On);
                }
                else
                {
                    await radio1.SetStateAsync(RadioState.Off);
                }
            }
            catch { }
        }
        private void LoadLayout()
        {
            try
            {
                string config = File.ReadAllText(Globals.TimeConfigFile);
                if (config != "")
                {
                    using StringReader readerconfig = new(config);
                    int RTime = int.Parse(readerconfig.ReadLine());
                    int GTime = int.Parse(readerconfig.ReadLine());
                    int BTime = int.Parse(readerconfig.ReadLine());
                    int RDate = int.Parse(readerconfig.ReadLine());
                    int GDate = int.Parse(readerconfig.ReadLine());
                    int BDate = int.Parse(readerconfig.ReadLine());
                    Time.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, (byte)RTime, (byte)GTime, (byte)BTime));
                    Date.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, (byte)RDate, (byte)GDate, (byte)BDate));
                }
            }
            catch { }
            try
            {
                string config2 = File.ReadAllText(Globals.TimeConfigFileQS);
                using StringReader readerconfig = new(config2);
                WiFiToggleGrid.Tag = readerconfig.ReadLine();
                BTToggleGrid.Tag = readerconfig.ReadLine();
                DarkModeToggleGrid.Tag = readerconfig.ReadLine();
                MuteToggleGrid.Tag = readerconfig.ReadLine();
                AirPlaneToggleGrid.Tag = readerconfig.ReadLine();
                ShutdownGrid.Tag = readerconfig.ReadLine();
                RestartGrid.Tag = readerconfig.ReadLine();
                SuspendGrid.Tag = readerconfig.ReadLine();
                LockGrid.Tag = readerconfig.ReadLine();
                LogoffGrid.Tag = readerconfig.ReadLine();
                var sortedOrderItems = TogglesView.Items.OrderBy(item => (item as Grid).Tag).ToList();
                // Clear the original collection and add the sorted items
                TogglesView.Items.Clear();
                foreach (var item in sortedOrderItems)
                {
                    TogglesView.Items.Add(item);
                }
            }
            catch { }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            loadCountPerUnload++;
            if (!isVisible)
            {
                isVisible = !isVisible;
                if (configChanged)
                {
                    LoadLayout();
                    LoadToggles();
                    configChanged = !configChanged;
                }
                try
                {
                    MusicAPI.CallInfoUpdate += SongUpdated;
                    MusicAPI.currentSession.PlaybackInfoChanged += PlayPauseUpdated;
                    mDevice.AudioEndpointVolume.OnVolumeNotification += ChangeVol;
                    radio1.StateChanged += UpdateWiFi;
                    radio2.StateChanged += UpdateBT;
                }
                catch { }
                SongUpdated();
                PlayPauseUpdated(null, null);
            }
        }

        private void PlayPauseUpdated(GlobalSystemMediaTransportControlsSession sender, PlaybackInfoChangedEventArgs args)
        {
            try
            {
                if (MusicAPI.playbackInfo.PlaybackStatus == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Paused)
                {
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        PlayPause.Content = "\uf5b0";
                    });
                }
                else
                {
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        PlayPause.Content = "\uf8ae";
                    });
                }
            }
            catch { }
        }

        private async void UpdateBL()
        {
            try
            {
                bluetoothDevices.Clear();
                bluetoothDevicesLE.Clear();
                DispatcherQueue.TryEnqueue(() =>
                {
                    BTStackBattery.Children.Clear();
                });
                var selector0 = BluetoothLEDevice.GetDeviceSelector();
                var devices0 = await DeviceInformation.FindAllAsync(selector0);
                foreach (var device in devices0)
                {
                    try
                    {
                        BluetoothLEDevice bleDevice = await BluetoothLEDevice.FromIdAsync(device.Id);
                        bleDevice.ConnectionStatusChanged += BLERefresh;
                        bluetoothDevicesLE.Add(bleDevice);
                        if (bleDevice.ConnectionStatus == BluetoothConnectionStatus.Connected)
                        {
                            DispatcherQueue.TryEnqueue(() =>
                            {
                                Frame frame = new()
                                {
                                    Name = device.Name,
                                    Tag = bleDevice
                                };
                                frame.Navigate(typeof(BTBatteryIndividual));
                                BTStackBattery.Children.Add(frame);
                            });
                        }
                    }
                    catch { }
                }
                var selector = BluetoothDevice.GetDeviceSelector();
                var devices = await DeviceInformation.FindAllAsync(selector);
                foreach (var device in devices)
                {
                    try
                    {
                        BluetoothDevice bleDevice0 = await BluetoothDevice.FromIdAsync(device.Id);
                        bluetoothDevices.Add(bleDevice0);
                        bleDevice0.ConnectionStatusChanged += BLRefresh;
                        if (bleDevice0.ConnectionStatus == BluetoothConnectionStatus.Connected)
                        {
                            DispatcherQueue.TryEnqueue(() =>
                            {
                                Frame frame = new()
                                {
                                    Name = device.Name,
                                };
                                frame.Navigate(typeof(BTBatteryIndividual));
                                BTStackBattery.Children.Add(frame);
                            });
                        }
                    }
                    catch { }
                }
            }
            catch
            {

            }
        }
        private async void LoadToggles()
        {
            try
            {
                uiSettings = new Windows.UI.ViewManagement.UISettings();
                var color = uiSettings.GetColorValue(Windows.UI.ViewManagement.UIColorType.Background);
                uiSettings.ColorValuesChanged += UpdateDarkMode;
                DarkModeToggle.IsChecked = color.R == 0;
                MMDeviceEnumerator DevEnum = new();
                mDevice = DevEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
                mDevice.AudioEndpointVolume.OnVolumeNotification += ChangeVol;
                MuteToggle.IsChecked = mDevice.AudioEndpointVolume.Mute;
                await Radio.RequestAccessAsync();
                var radios = await Radio.GetRadiosAsync();
                foreach (var radio in radios)
                {
                    if (radio.Kind == RadioKind.WiFi)
                    {
                        WiFiToggle.IsEnabled = true;
                        WiFiToggle.IsHitTestVisible = true;
                        radio1 = radio;
                        radio1.StateChanged -= UpdateWiFi;
                        radio1.StateChanged += UpdateWiFi;
                        WiFiToggle.IsChecked = (radio.State == RadioState.On);
                    }
                    if (radio.Kind == RadioKind.Bluetooth)
                    {
                        BTToggle.IsEnabled = true;
                        BTToggle.IsHitTestVisible = BTToggle.IsEnabled;
                        radio2 = radio;
                        radio2.StateChanged -= UpdateBT;
                        radio2.StateChanged += UpdateBT;
                        BTToggle.IsChecked = (radio.State == RadioState.On);
                    }
                }
                GetAirplaneMode();
                UpdateBL();
            }
            catch
            {

            }
        }
        private void BLRefresh(BluetoothDevice sender, object args)
        {
            foreach(BluetoothDevice bluetoothDevice in bluetoothDevices)
            {
                bluetoothDevice.ConnectionStatusChanged -= BLRefresh;
            }
            foreach (BluetoothLEDevice bluetoothDevice in bluetoothDevicesLE)
            {
                bluetoothDevice.ConnectionStatusChanged -= BLERefresh;
            }
            sender.ConnectionStatusChanged -= BLRefresh;
            UpdateBL();
        }
        private readonly List<BluetoothDevice> bluetoothDevices = [];
        private readonly List<BluetoothLEDevice> bluetoothDevicesLE = [];
        private void BLERefresh(BluetoothLEDevice sender, object args)
        {
            sender.ConnectionStatusChanged -= BLERefresh;
            UpdateBL();
        }

        private void ChangeVol(AudioVolumeNotificationData data)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                MuteToggle.IsChecked = data.Muted;
            });
        }

        private MMDevice mDevice;
        private UISettings uiSettings;

        private void DarkModeToggle_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if(DarkModeToggle.IsChecked.Value)
            {
                CommandAPI.PerformPowershellCommand($"-Command \"New-ItemProperty -Path HKCU:\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize -Name AppsUseLightTheme -Value 0 -Type Dword -Force");
            }
            else
            {
                CommandAPI.PerformPowershellCommand($"-Command \"Remove-ItemProperty -Path HKCU:\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize -Name AppsUseLightTheme\"");
            }
        }

        private void MuteToggle_Tapped(object sender, TappedRoutedEventArgs e)
        {
            mDevice.AudioEndpointVolume.Mute = MuteToggle.IsChecked.Value;
        }

        private async void HyperlinkButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                await MusicAPI.currentSession.TrySkipNextAsync();
            }
            catch { }
        }

        private async void PlayPause_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                await MusicAPI.currentSession.TryTogglePlayPauseAsync();
            }
            catch { }
        }

        private async void Shutdown_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ContentDialog dialog = new()
            {
                XamlRoot = this.XamlRoot,
                Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                Title = AppStrings.GetActionString((sender as Button).Name),
                PrimaryButtonText = AppStrings.GetActionString((sender as Button).Name),
                CloseButtonText = AppStrings.cancelString,
                DefaultButton = ContentDialogButton.Primary,
                Content = AppStrings.doYouWanna + AppStrings.GetActionString((sender as Button).Name) + AppStrings.yourPC,
                Tag = (sender as Button).Tag
            };
            var result = await dialog.ShowAsync();
            if(result == ContentDialogResult.Primary)
            {
                CommandAPI.PerformCMDCommand((string)(sender as Button).Tag);
            }
        }

        private void Edit_Checked(object sender, RoutedEventArgs e)
        {
            Edit.Content = "\uf13e";
            foreach (Grid grid in TogglesView.Items.Cast<Grid>())
            {
                foreach (Control childControl in grid.Children.Cast<Control>())
                {
                    childControl.IsHitTestVisible = false;
                }
            }
        }

        private void Edit_Unchecked(object sender, RoutedEventArgs e)
        {
            Edit.Content = "\ue70f";
            int index = 0;
            foreach (Grid grid in TogglesView.Items.Cast<Grid>())
            {
                foreach (Control childControl in grid.Children.Cast<Control>())
                {
                    childControl.IsHitTestVisible = true;
                }
                grid.Tag = index;
                index++;
            }
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(Globals.TimeConfigFileQS));
            File.Delete(Globals.TimeConfigFileQS);
            File.AppendAllText(Globals.TimeConfigFileQS, WiFiToggleGrid.Tag + "\n");
            File.AppendAllText(Globals.TimeConfigFileQS, BTToggleGrid.Tag + "\n");
            File.AppendAllText(Globals.TimeConfigFileQS, DarkModeToggleGrid.Tag + "\n");
            File.AppendAllText(Globals.TimeConfigFileQS, MuteToggleGrid.Tag + "\n");
            File.AppendAllText(Globals.TimeConfigFileQS, AirPlaneToggleGrid.Tag + "\n");
            File.AppendAllText(Globals.TimeConfigFileQS, ShutdownGrid.Tag + "\n");
            File.AppendAllText(Globals.TimeConfigFileQS, RestartGrid.Tag + "\n");
            File.AppendAllText(Globals.TimeConfigFileQS, SuspendGrid.Tag + "\n");
            File.AppendAllText(Globals.TimeConfigFileQS, LockGrid.Tag + "\n");
            File.AppendAllText(Globals.TimeConfigFileQS, LogoffGrid.Tag + "\n");
        }
        private void GetAirplaneMode()
        {
            bool output = CommandAPI.GetPowershellLog($"-Command \"(Get-ItemProperty -Path \"HKLM:\\System\\CurrentControlSet\\Control\\RadioManagement\\SystemRadioState\").'(default)'").Contains('1');
            DispatcherQueue.TryEnqueue(() =>
            {
                AirPlaneToggle.IsChecked = output;
            });
        }

        private void UpdateDarkMode(UISettings sender, object args)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                DarkModeToggle.IsChecked = sender.GetColorValue(Windows.UI.ViewManagement.UIColorType.Background).R == 0;
            });
        }

        private void UpdateBT(Radio sender, object args)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                BTToggle.IsChecked = sender.State == RadioState.On;
            });
            GetAirplaneMode();
        }
        private void UpdateWiFi(Radio sender, object args)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                WiFiToggle.IsChecked = sender.State == RadioState.On;
            });
            GetAirplaneMode();
        }
    }
}
