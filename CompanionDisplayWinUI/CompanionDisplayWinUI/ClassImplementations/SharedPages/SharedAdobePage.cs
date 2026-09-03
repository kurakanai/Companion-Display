using CompanionDisplayWinUI.API;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace CompanionDisplayWinUI.ClassImplementations.SharedPages
{
    public partial class SharedAdobePage : Page
    {
        protected void PressKeyCTRL(object sender, TappedRoutedEventArgs _1)
        {
            KeyPressAPI.CallKeys(int.Parse((string)(sender as Button).Tag), 17);
        }

        protected void PressKeyNoModifiers(object sender, TappedRoutedEventArgs _1)
        {
            KeyPressAPI.CallKeys(int.Parse((string)(sender as Button).Tag), -1);
        }
    }
}
