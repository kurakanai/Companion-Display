using CompanionDisplayWinUI.ClassImplementations;
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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI.Pages.Widgets.OBS.SharedAssets;

public sealed partial class OBSUnavailableOverlay : UserControl
{
    public OBSUnavailableOverlay()
    {
        InitializeComponent();
    }
    private void Button_Tapped(object sender, TappedRoutedEventArgs _1)
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

}
