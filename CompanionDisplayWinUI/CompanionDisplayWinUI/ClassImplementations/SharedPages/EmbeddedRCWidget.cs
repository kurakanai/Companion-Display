using CompanionDisplayWinUI.Objects;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CompanionDisplayWinUI.ClassImplementations.SharedPages
{
    public class EmbeddedRCWidget : CommonWidget
    {
        public bool selfRC = true;
        internal void triggerRightClick(MenuFlyoutItem[] items, FrameworkElement senderElement)
        {
            MenuFlyout myFlyout = new();
            for (int i = 0; i < items.Length; i++)
            {
                myFlyout.Items.Add(items[i]);
            }
            myFlyout.ShowAt(senderElement, new Windows.Foundation.Point(0, 0));
        }
    }
}
