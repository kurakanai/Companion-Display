using CompanionDisplayWinUI.API;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace CompanionDisplayWinUI.ClassImplementations
{
    public class SharedAdobePage : Page
    {
        protected void PressKeyCTRL(object sender, TappedRoutedEventArgs e)
        {
            KeyPressAPI.callKeys(int.Parse((string)(sender as Button).Tag), 17);
        }

        protected void PressKeyNoModifiers(object sender, TappedRoutedEventArgs e)
        {
            KeyPressAPI.callKeys(int.Parse((string)(sender as Button).Tag), -1);
        }
    }
}
