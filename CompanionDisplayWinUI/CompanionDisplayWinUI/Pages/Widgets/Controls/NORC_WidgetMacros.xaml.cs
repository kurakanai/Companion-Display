using CompanionDisplayWinUI.API;
using CompanionDisplayWinUI.Objects;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI.ClassImplementations.SharedPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NORC_WidgetMacros : EmbeddedRCWidget
    {
        public NORC_WidgetMacros()
        {
            this.InitializeComponent();
        }
        private void Button_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            FrameworkElement senderElement = sender as FrameworkElement;
            MenuFlyoutItem[] flyoutRC =
            [
                new() { Text = AppStrings.replaceImage, Name = senderElement.Name + "ACTION1" },
                new() { Text = AppStrings.removeImage, Name = senderElement.Name + "ACTION2" },
            ];
            flyoutRC[0].Click += ReplaceImageClick;
            flyoutRC[1].Click += RemoveImageClick;
            TriggerRightClick(flyoutRC, senderElement);

        }
        private void MainGrid_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            FrameworkElement senderElement = sender as FrameworkElement;
            MenuFlyoutItem[] flyoutRC = 
            [
                new(){ Text = AppStrings.removeWidget, Name = senderElement.Name + "Flyout" },
                new(){ Text = AppStrings.widgetPinUnpin, Name = senderElement.Name + "Pin" },
                new(){ Text = AppStrings.pipOpen, Name = senderElement.Name + "PiP" },
            ];
            flyoutRC[0].Click += MenuFlyoutItem_Click;
            flyoutRC[1].Click += PinButton;
            flyoutRC[2].Click += PiPButton;
            TriggerRightClick(flyoutRC, senderElement);
        }
        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            var frame = this.Parent as CommonWidgetContainer;
            frame.TriggerRightClickFromChild("");
        }

        private void PiPButton(object sender, RoutedEventArgs e)
        {
            var frame = this.Parent as CommonWidgetContainer;
            frame.TriggerRightClickFromChild("pip");
        }
        private void PinButton(object sender, RoutedEventArgs e)
        {
            var frame = this.Parent as CommonWidgetContainer;
            frame.TriggerRightClickFromChild("pin");
        }

        private void RemoveImageClick(object sender, RoutedEventArgs e)
        {
            FrameworkElement senderElement = sender as FrameworkElement;
            var childControl = (Microsoft.UI.Xaml.Controls.Image)MainGrid.FindName(senderElement.Name[..^7] + "_Image");
            childControl.Tag = "";
            childControl.Source = null;
            ((childControl.Parent as Grid).Children[0] as Microsoft.UI.Xaml.Controls.TextBlock).Visibility = Visibility.Visible;
            SaveItems();
        }

        private void ReplaceImageClick(object sender, RoutedEventArgs e)
        {
            FrameworkElement senderElement = sender as FrameworkElement;
            var childControl = (Microsoft.UI.Xaml.Controls.Image)MainGrid.FindName(senderElement.Name[..^7] + "_Image");
            string btntag = FileAPI.OpenFileDialog(false)[0];
            if (btntag != "")
            {
                childControl.Tag = btntag;
                childControl.Source = new BitmapImage(new Uri(childControl.Tag.ToString()));
                ((childControl.Parent as Grid).Children[0] as Microsoft.UI.Xaml.Controls.TextBlock).Visibility = Visibility.Collapsed;
            }
            SaveItems();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            int i = 1;
            string LoadImages = "";
            try
            {
                LoadImages = File.ReadAllText("Config/MacroThumbs.crlh");
            }
            catch
            {
                Directory.CreateDirectory("Config");
                File.AppendAllText("Config/MacroThumbs.crlh", "");
            }
            foreach (string line in LoadImages.Replace("\r", "").Split('\n'))
            {
                try
                {
                    (((MainGrid.Children[i] as Button).Content as Grid).Children[1] as Microsoft.UI.Xaml.Controls.Image).Tag = line;
                    (((MainGrid.Children[i] as Button).Content as Grid).Children[1] as Microsoft.UI.Xaml.Controls.Image).Source = new BitmapImage(new Uri((((MainGrid.Children[i] as Button).Content as Grid).Children[1] as Microsoft.UI.Xaml.Controls.Image).Tag.ToString()));
                    (((MainGrid.Children[i] as Button).Content as Grid).Children[0] as Microsoft.UI.Xaml.Controls.TextBlock).Visibility = Visibility.Collapsed;
                }
                catch
                {
                    try
                    {
                        (((MainGrid.Children[i] as Button).Content as Grid).Children[0] as Microsoft.UI.Xaml.Controls.TextBlock).Visibility = Visibility.Visible;
                    }
                    catch
                    {

                    }
                }
                i++;
            }
        }
        private void SaveItems()
        {
            string finalFile = "";
            foreach(IVisualElement visualElement in MainGrid.Children)
            {
                if(visualElement as Button != null)
                {
                    try
                    {
                        finalFile = finalFile + (((visualElement as Button).Content as Grid).Children[1] as Microsoft.UI.Xaml.Controls.Image).Tag.ToString() + Environment.NewLine;
                    }
                    catch
                    {
                        finalFile += Environment.NewLine;
                    }
                }
            }
            File.Delete("Config/MacroThumbs.crlh");
            File.AppendAllText("Config/MacroThumbs.crlh", finalFile);
        }
        public static async Task<BitmapImage> GetThumbnailImageAsync(string filePath)
        {
            BitmapImage bitmapImage = new();
            StorageFile storageFile = await StorageFile.GetFileFromPathAsync(filePath);
            const ThumbnailMode thumbnailMode = ThumbnailMode.PicturesView;
            const uint requestedSize = 100; // Size of the thumbnail
            using (StorageItemThumbnail thumbnail = await storageFile.GetThumbnailAsync(thumbnailMode, requestedSize))
            {
                bitmapImage.SetSource(thumbnail);
            }
            return bitmapImage;
        }
        [LibraryImport("user32.dll", SetLastError = true)]
        static partial void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);
        public static void PressKey(VirtualKey key, bool up)
        {
            const int KEYEVENTF_EXTENDEDKEY = 0x1;
            const int KEYEVENTF_KEYUP = 0x2;
            if (up)
                keybd_event((byte)key, 0x45, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, (UIntPtr)0);
            else
                keybd_event((byte)key, 0x45, KEYEVENTF_EXTENDEDKEY, (UIntPtr)0);
        }
        private void F13Btn_Tapped(object sender, TappedRoutedEventArgs e)
        {
            PressKey((VirtualKey)int.Parse(((Button)sender).Tag.ToString()), up: false);
            PressKey((VirtualKey)int.Parse(((Button)sender).Tag.ToString()), up: true);
        }
    }
}
