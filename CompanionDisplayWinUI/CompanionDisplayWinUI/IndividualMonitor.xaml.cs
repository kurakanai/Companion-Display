using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class IndividualMonitor : Page
    {
        public IndividualMonitor()
        {
            this.InitializeComponent();
        }
        private bool FTU = true;
        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            if (FTU)
            {
                FTU = !FTU;
                Frame parentFrame = GetParentFrame();
                if (parentFrame != null)
                {
                    if (parentFrame.Name.Length > 14)
                    {
                        MonitorName.Text = parentFrame.Name[..10] + "...";
                    }
                    else
                    {
                        MonitorName.Text = parentFrame.Name;
                    }
                    GetMonitorBrightness(nint.Parse(parentFrame.Tag as string), out uint minBrightness, out uint currentBrightness, out uint maxBrightness);
                    MonitorBrightness.Value = currentBrightness;
                    MonitorBrightness.Maximum = maxBrightness;
                    MonitorBrightness.Minimum = minBrightness;
                    if (maxBrightness <= 0)
                    {
                        var GridElementMain = parentFrame.Parent as GridView;
                        GridElementMain.Items.Remove(parentFrame);
                    }
                }
                else
                {

                }
            }
        }
        public Frame GetParentFrame()
        {
            var parent = this.Parent as Frame;
            return parent;
        }
        
        private void BrightnessSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            int brightness = (int)e.NewValue;
            SetMonitorBrightness(nint.Parse((GetParentFrame().Tag as string)), (uint)brightness);
        }
        [DllImport("dxva2.dll", SetLastError = true)]
        private static extern bool SetMonitorBrightness(IntPtr hMonitor, uint dwNewBrightness);
        [DllImport("dxva2.dll", SetLastError = true)]
        private static extern bool GetMonitorBrightness(IntPtr hMonitor, out uint pdwMinimumBrightness, out uint pdwCurrentBrightness, out uint pdwMaximumBrightness);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct PHYSICAL_MONITOR
        {
            public IntPtr hPhysicalMonitor;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string szPhysicalMonitorDescription;
        }
    }
}
