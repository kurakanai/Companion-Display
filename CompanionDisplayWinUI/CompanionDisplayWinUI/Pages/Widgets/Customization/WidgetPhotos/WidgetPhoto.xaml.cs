using System;
using System.IO;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Threading;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WidgetPhoto : Page
    {
        public WidgetPhoto()
        {
            this.InitializeComponent();
        }
        Frame frame;
        string path = "";
        int ItemCount = 0, ChosenImage = 0;
        public void UpdateUI()
        {
            try
            {
                Globals.SmartFlipToggle = bool.Parse(File.ReadAllText(Globals.PhotoConfigFile));
            }
            catch
            {

            }
            int pos = path.LastIndexOf('/') + 1;
            if (path[pos..].Contains('.'))
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    Image image = new();
                    BitmapImage bitmapImage = new();
                    using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                    {
                        bitmapImage.SetSource(stream.AsRandomAccessStream());
                    }
                    image.Source = bitmapImage;
                    image.Stretch = Stretch.UniformToFill;
                    image.Name = path;
                    FlipViewImages.Items.Add(image);
                });
            }
            else
            {
                try
                {
                    DirectoryInfo d = new(path);
                    foreach (var file in d.GetFiles("*"))
                    {
                        string imagePath = file.FullName;
                        if (MimeMapping.MimeUtility.GetMimeMapping(imagePath).StartsWith("image/"))
                        {
                            DispatcherQueue.TryEnqueue(() =>
                            {
                                try
                                {
                                    Image image = new()
                                    {
                                        Stretch = Stretch.UniformToFill,
                                        Name = imagePath
                                    };
                                    image.Loaded += Image_Loaded;
                                    image.Unloaded += Image_Unloaded;
                                    FlipViewImages.Items.Add(image);
                                    ItemCount++;
                                }
                                catch
                                {
                                }
                            });
                        }
                    }
                }
                catch { }
            }
            Thread thread = new(SmartFlip);
            thread.Start();
        }
        private readonly BitmapImage bitmapImage = new();
        private bool IsVisible = false;
        private void SmartFlip()
        {
            while (Globals.SmartFlipToggle && IsVisible)
            {
                Random random = new();
                ChosenImage = random.Next(0, ItemCount);
                DispatcherQueue.TryEnqueue(() =>
                {
                    FlipViewImages.SelectedIndex = ChosenImage;
                });
                Thread.Sleep(60000);
            }
        }
        private bool FTU = true;
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (FTU)
            {
                frame = this.Parent as Frame;
                try
                {
                    path = frame.Tag.ToString();
                }
                catch { }
                Thread thread = new(UpdateUI);
                thread.Start();
                FTU = false;
            }
            IsVisible = true;
            Thread thread0 = new(SmartFlip);
            thread0.Start();
        }

        private void Image_Loaded(object sender, RoutedEventArgs e)
        {
            FlipViewImages.IsEnabled = false;
            try
            {
                var senderImage = sender as Image;
                senderImage.Stretch = Stretch.UniformToFill;
                senderImage.Source = bitmapImage;
            }
            catch
            {

            }
            FlipViewImages.IsEnabled = true;
        }
        private void Image_Unloaded(object sender, RoutedEventArgs e)
        {
            if (IsVisible)
            {
                var senderImage = sender as Image;
                if (senderImage != null)
                {
                    string oldname = senderImage.Name;
                    senderImage = null;
                    senderImage = new()
                    {
                        Name = oldname
                    };
                    senderImage.Loaded += Image_Loaded;
                }
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            IsVisible = false;
        }

        private void FlipViewImages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var senderImage = (sender as FlipView).SelectedItem as Image;
                bitmapImage.UriSource = new System.Uri(senderImage.Name);
            }
            catch
            {

            }
        }
    }
}
