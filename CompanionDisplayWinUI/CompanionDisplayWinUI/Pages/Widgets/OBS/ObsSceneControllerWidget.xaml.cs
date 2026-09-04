using CompanionDisplayWinUI.ClassImplementations;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using OBSWebsocketDotNet.Communication;
using System;
using System.Threading;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ObsSceneControllerWidget : Page
    {
        public ObsSceneControllerWidget()
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
            Globals.obsControls.CallUpdate -= UpdateUI;
            DispatcherQueue.TryEnqueue(() =>
            {
                OBSError.Visibility = Visibility.Visible;
            });
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
                Page_Unloaded(null, null);
                Page_Loaded(null, null);
            });
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (Globals.obsControls.connectionSuccessful)
            {
                UpdateUI();
                Globals.obsControls.CallUpdate += UpdateUI;
            }
            else
            {
                OBSError.Visibility = Visibility.Visible;
            }
        }

        private void UpdateUI()
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                try
                {
                    TotalScenes.Items.Clear();
                    for (int i = 0; i < Globals.obsControls.scenes.Length; i++)
                    {
                        Grid grid = new()
                        {
                            Width = 90,
                            Height = 90
                        };
                        TextBlock individualScene = new()
                        {
                            Text = Globals.obsControls.scenes[i].Name,
                            TextWrapping = TextWrapping.WrapWholeWords,
                            VerticalAlignment = VerticalAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Center
                        };
                        grid.Children.Add(individualScene);
                        TotalScenes.Items.Insert(0, grid);
                        if (Globals.obsControls.scenes[i].Name == Globals.obsControls.currentSession)
                        {
                            TotalScenes.SelectedIndex = 0;
                            if (Application.Current.Resources.TryGetValue("SystemAccentColor", out var accentColorObj) && accentColorObj is Windows.UI.Color accentColor)
                            {
                                // Apply the brush to the rectangle's Fill property
                                grid.Background = new SolidColorBrush(accentColor);
                            }
                        }
                    }
                }
                catch
                {

                }
            });
        }

        private void TotalScenes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Globals.obsControls.SetScene(((TotalScenes.SelectedItem as Grid).Children[0] as TextBlock).Text);
            }
            catch
            {

            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            Globals.obsControls.CallUpdate -= UpdateUI;
        }
    }
}
