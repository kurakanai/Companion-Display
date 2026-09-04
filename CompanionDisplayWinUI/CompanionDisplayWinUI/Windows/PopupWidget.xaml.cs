using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Runtime.InteropServices;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PopupWidget : Window
    {
        public PopupWidget(Frame widget, GridView gridViewPiPTemp0, Grid placeholder0)
        {
            gridViewPiPTemp = gridViewPiPTemp0;
            placeholder = placeholder0;
            this.InitializeComponent();
            WidgetGrid.Children.Add(widget);
            widgetFrame = widget;
            this.AppWindow.Resize(new Windows.Graphics.SizeInt32 { Width = (int)(514 * GetRenderScale()), Height = (int)(309 * GetRenderScale())});
            this.ExtendsContentIntoTitleBar = true;
            CustomizeWindowStyle();
        }
        public Frame widgetFrame;
        private double GetRenderScale()
        {
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            uint dpi = GetDpiForWindow(hwnd);
            return(dpi / 96.0);
        }

        [DllImport("User32.dll")]
        private static extern uint GetDpiForWindow(IntPtr hWnd);
        private void CustomizeWindowStyle()
        {
            // Get the Window handle (HWND)
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);

            // Get the current window style
            var style = GetWindowLong(hwnd, GWL_STYLE);

            // Remove resizable, maximize, and minimize flags
            style &= ~(WS_SIZEBOX | WS_MAXIMIZEBOX | WS_MINIMIZEBOX);

            // Apply the updated style
            _ = SetWindowLong(hwnd, GWL_STYLE, style);
            
        }

        // Constants for Window Styles
        private const int GWL_STYLE = -16;
        private const int WS_SIZEBOX = 0x00040000;   // Enables resizing
        private const int WS_MAXIMIZEBOX = 0x00010000; // Enables maximize button
        private const int WS_MINIMIZEBOX = 0x00020000; // Enables minimize button
        private static readonly IntPtr HWND_TOPMOST = new(-1);
        private static readonly IntPtr HWND_NOTOPMOST = new(-2);
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_NOSIZE = 0x0001;

        // Import SetWindowLong and GetWindowLong from User32.dll
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(
            IntPtr hWnd,
            IntPtr hWndInsertAfter,
            int X,
            int Y,
            int cx,
            int cy,
            uint uFlags);
        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            SetWindowPos(hwnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            SetWindowPos(hwnd, HWND_NOTOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
        }
        public GridView gridViewPiPTemp;
        public Grid placeholder;

        private void WidgetGrid_Unloaded(object sender, RoutedEventArgs e)
        {
            if (gridViewPiPTemp != null)
            {
                int placeholderIndex = gridViewPiPTemp.Items.IndexOf(placeholder);
                gridViewPiPTemp.Items.RemoveAt(placeholderIndex);
                gridViewPiPTemp.Items.Insert(placeholderIndex, widgetFrame);
            }
        }

        private void Window_Closed(object sender, WindowEventArgs args)
        {
            WidgetGrid.Children.Clear();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            GetRenderScale();
        }
    }
}
