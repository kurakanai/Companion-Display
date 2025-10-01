using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CompanionDisplayWinUI
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class SetupStep3 : Page
	{
		public SetupStep3()
		{
			this.InitializeComponent();
		}

        private void Button_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var frame = Parent as Frame;
            var navviewparent = frame.Parent as NavigationView;
            frame.GoBack();
        }

        private void Button_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            var frame = Parent as Frame;
            var navviewparent = frame.Parent as NavigationView;
            frame.Navigate(typeof(SetupStep4), null, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Globals.SleepColorR = SleepModeColor.Color.R;
            Globals.SleepColorG = SleepModeColor.Color.G;
            Globals.SleepColorB = SleepModeColor.Color.B;
            Globals.sleepModeOpacity = OpacitySlider.Value;
            Globals.OverrideColor = OvrColorSleepMode.IsChecked.Value;
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            TimeDemo.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, (byte)Globals.SleepColorR, (byte)Globals.SleepColorG, (byte)Globals.SleepColorB));
            DateDemo.Foreground = TimeDemo.Foreground;
            OvrColorSleepMode.IsChecked = (bool)Globals.OverrideColor;
        }
        private void SleepModeColor_ColorChanged(ColorPicker sender, ColorChangedEventArgs args)
        {
            TimeDemo.Foreground = new SolidColorBrush(sender.Color);
            DateDemo.Foreground = TimeDemo.Foreground;
        }

        private void Opacity_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            DemoUnderflow.Opacity = (float)(e.NewValue/100) ;
        }

        private void HyperlinkButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var frame = this.Parent as Frame;
            var navviewparent = frame.Parent as NavigationView;
            frame.Navigate(typeof(SetupStep4), null, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
        }
    }
}
