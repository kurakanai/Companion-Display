using CompanionDisplayWinUI.ClassImplementations;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using OBSWebsocketDotNet.Communication;
using OBSWebsocketDotNet.Types.Events;
using System;
using System.Threading;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ObsStreamControlsWidget : Page
    {
        public ObsStreamControlsWidget()
        {
            this.InitializeComponent();
            Globals.obsControls.obs.Disconnected += DisconnectEvent;
            Globals.obsControls.obs.Connected += ConnectedEvent;
        }

        private void ConnectedEvent(object sender, EventArgs e)
        {
            RevertLayout();
        }

        private void DisconnectEvent(object sender, ObsDisconnectionInfo e)
        {
            Globals.obsControls.obs.StreamStateChanged -= UpdateStreamChecker;
            Globals.obsControls.obs.RecordStateChanged -= UpdateRecordChecker;
            Globals.obsControls.obs.ReplayBufferStateChanged -= UpdateBufferChecker;
            Globals.obsControls.obs.VirtualcamStateChanged -= CamChecker;
            DispatcherQueue.TryEnqueue(() =>
            {
                OBSError.Visibility = Visibility.Visible;
            });
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (Globals.obsControls.connectionSuccessful)
            {
                try
                {
                    Globals.obsControls.obs.StreamStateChanged += UpdateStreamChecker;
                    Globals.obsControls.obs.RecordStateChanged += UpdateRecordChecker;
                    Globals.obsControls.obs.ReplayBufferStateChanged += UpdateBufferChecker;
                    Globals.obsControls.obs.VirtualcamStateChanged += CamChecker;
                    RecordButton.IsChecked = Globals.obsControls.obs.GetRecordStatus().IsRecording || Globals.obsControls.obs.GetRecordStatus().IsRecordingPaused;
                    PauseButton.IsChecked = Globals.obsControls.obs.GetRecordStatus().IsRecordingPaused;
                    PauseButton.IsEnabled = RecordButton.IsChecked.Value;
                    StreamButton.IsChecked = Globals.obsControls.obs.GetStreamStatus().IsActive;
                    CameraButton.IsChecked = Globals.obsControls.obs.GetVirtualCamStatus().IsActive;
                    ToggleBuffer.IsChecked = Globals.obsControls.obs.GetReplayBufferStatus();
                    BufferSave.IsEnabled = ToggleBuffer.IsChecked.Value;
                }
                catch
                {
                }
            }
            else
            {
                OBSError.Visibility = Visibility.Visible;
            }
        }

        private void CamChecker(object sender, VirtualcamStateChangedEventArgs e)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                CameraButton.IsChecked = e.OutputState.IsActive;
            });
        }

        private void UpdateBufferChecker(object sender, ReplayBufferStateChangedEventArgs e)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                ToggleBuffer.IsChecked = Globals.obsControls.obs.GetReplayBufferStatus();
                BufferSave.IsEnabled = ToggleBuffer.IsChecked.Value;
            });
        }

        private void UpdateRecordChecker(object sender, RecordStateChangedEventArgs e)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                RecordButton.IsChecked = (e.OutputState.IsActive || e.OutputState.State == OBSWebsocketDotNet.Types.OutputState.OBS_WEBSOCKET_OUTPUT_PAUSED);
                PauseButton.IsEnabled = RecordButton.IsChecked.Value;
                PauseButton.IsChecked = e.OutputState.State == OBSWebsocketDotNet.Types.OutputState.OBS_WEBSOCKET_OUTPUT_PAUSED;
            });
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            Globals.obsControls.obs.StreamStateChanged -= UpdateStreamChecker;
            Globals.obsControls.obs.RecordStateChanged -= UpdateRecordChecker;
            Globals.obsControls.obs.ReplayBufferStateChanged -= UpdateBufferChecker;
            Globals.obsControls.obs.VirtualcamStateChanged -= CamChecker;
        }
        private static void ActionSelector(ToggleButton sender, bool start)
        {
            try
            {
                switch (sender.Tag)
                {
                    case "Streaming":
                        if (start)
                        {
                            Globals.obsControls.StartStreaming();
                        }
                        else
                        {
                            Globals.obsControls.StopStreaming();
                        }
                        break;
                    case "Recording":
                        if (start)
                        {
                            Globals.obsControls.StartRecording();
                        }
                        else
                        {
                            Globals.obsControls.StopRecording();
                        }
                        break;
                }
            }
            catch
            {
                (sender as ToggleButton).IsChecked = !(sender as ToggleButton).IsChecked;
            }
            
        }
        private void UpdateStreamChecker(object sender, StreamStateChangedEventArgs e)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                StreamButton.IsChecked = e.OutputState.IsActive;
            });
        }

        private async void StreamButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ContentDialog dialog = new()
            {
                XamlRoot = this.XamlRoot,
                Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style
            };
            string tof = AppStrings.stop;
            if ((sender as ToggleButton).IsChecked.Value)
            {
                tof = AppStrings.start;
            }
            dialog.Title = AppStrings.doYouWanna + tof + (sender as ToggleButton).Tag + "?";
            dialog.PrimaryButtonText = tof + (sender as ToggleButton).Tag.ToString();
            dialog.CloseButtonText = AppStrings.cancelString;
            dialog.DefaultButton = ContentDialogButton.Primary;
            dialog.Content = AppStrings.confirmOBS;
            dialog.Tag = (sender as ToggleButton).Tag;
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                ActionSelector(sender as ToggleButton, (sender as ToggleButton).IsChecked.Value);
            }
            (sender as ToggleButton).IsChecked = !(sender as ToggleButton).IsChecked;
        }

        private void ToggleButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Globals.obsControls.PauseToggle();
        }

        private void CameraButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Globals.obsControls.CameraToggle();
        }

        private void MicButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Globals.obsControls.MicToggle();
            MicButton.IsChecked = Globals.obsControls.micMute;
        }

        private void ToggleBuffer_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                Globals.obsControls.BufferToggle();
            }
            catch
            {
                ToggleBuffer.IsChecked = false;
                System.Timers.Timer timer = new(3000) { Enabled = true };
                ToggleBuffer.Content = "";
                timer.Elapsed += (sender, args) =>
                {
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        ToggleBuffer.Content = "";
                        timer.Dispose();
                    });
                };
            }
        }

        private void BufferSave_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                Globals.obsControls.BufferSave();
                
                System.Timers.Timer timer = new(3000) { Enabled = true };
                BufferSave.Content = "";
                timer.Elapsed += (sender, args) =>
                {
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        BufferSave.Content = "";
                        timer.Dispose();
                    });
                };
            }
            catch
            {
                System.Timers.Timer timer = new(3000) { Enabled = true };
                BufferSave.Content = "";
                timer.Elapsed += (sender, args) =>
                {
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        BufferSave.Content = "";
                        timer.Dispose();
                    });
                };
            }
            
        }

        private void Button_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Thread thread = new(Globals.obsControls.ManualConnectReq);
            thread.Start();
            System.Timers.Timer timer = new(3000) { Enabled = true };
            (sender as Button).IsEnabled = false;
            timer.Elapsed += (sender, args) =>
            {
                if (!Globals.obsControls.connectionSuccessful)
                {
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        ReconnectBtn.IsEnabled = true;
                        ReconnectBtn.Content = AppStrings.obsConnectionFailed;
                        timer.Dispose();
                    });
                }
            };
        }

        private void RevertLayout()
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                OBSError.Visibility = Visibility.Collapsed;
                Page_Loaded(null, null);
            });
        }
    }
}
