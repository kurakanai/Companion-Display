using CompanionDisplayWinUI.API;
using CompanionDisplayWinUI.ClassImplementations;
using CompanionDisplayWinUI.Objects;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
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

namespace CompanionDisplayWinUI.AppDesign.ArduinoElements
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ArduinoInterfaceHomePage : Page
    {
        public ArduinoInterfaceHomePage()
        {
            InitializeComponent();
            MusicAPI.CallInfoUpdate += UpdateUI;
            DataContext = Globals.publicTimeViewModel;
        }
        private static readonly SemaphoreSlim semaphore = new(1);
        SongObject songObject = MusicAPI.currentSong;
        private void UpdateUI()
        {
            songObject = MusicAPI.currentSong;
            DispatcherQueue.TryEnqueue(() =>
            {
                SongName.Text = songObject.title;
                SongDetails.Text = MusicAPI.buildDetails();
            });
        }
        private async void SongName_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            semaphore.WaitAsync();
            TextBlock senderBlock = sender as TextBlock;
            AnimateSideways((sender as TextBlock).RenderTransform as TranslateTransform, sender as TextBlock);
            semaphore.Release();
        }
        private void AnimateSideways(TranslateTransform translateTransform, TextBlock target)
        {
            double containerWidth = SongStack.ActualWidth;
            double textWidth = target.ActualWidth;
            double maxOffset = textWidth - containerWidth;
            Storyboard sb0 = target.Tag as Storyboard;
            if (sb0 == null)
            {
                sb0 = new Storyboard();
                target.Tag = sb0;
                var anim = new DoubleAnimation
                {
                    From = 0,
                    To = -maxOffset,
                    Duration = TimeSpan.FromSeconds(5),
                    AutoReverse = true,
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut },
                    RepeatBehavior = RepeatBehavior.Forever,
                };
                Storyboard.SetTarget(anim, translateTransform);
                Storyboard.SetTargetProperty(anim, "X");
                sb0.Children.Add(anim);
            }
            DoubleAnimation doubleAnimation = sb0.Children[0] as DoubleAnimation;
            if (maxOffset > 0)
            {
                doubleAnimation.To = -maxOffset;
                sb0.Begin();
            }
            else
            {
                sb0.Stop();
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            MusicAPI.CallInfoUpdate -= UpdateUI;
        }
    }
}
