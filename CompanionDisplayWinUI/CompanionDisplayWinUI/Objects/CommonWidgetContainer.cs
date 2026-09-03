using CompanionDisplayWinUI.ClassImplementations;
using CompanionDisplayWinUI.ClassImplementations.SharedPages;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading;

namespace CompanionDisplayWinUI.Objects
{
    public partial class CommonWidgetContainer : Frame
    {
        public CommonWidgetContainer(Type targetWidget)
        {
            this.Navigate(targetWidget);
            if (this.Content is not EmbeddedRCWidget)
            {
                this.RightTapped += CommonlyAccessedInstances.blankPage1.Frame_RightTapped;
            }
        }
        internal void TriggerRightClickFromChild(string args)
        {
            switch (args)
            {
                case "pin":
                    CommonlyAccessedInstances.blankPage1.Pin_Click_NC(this, null);
                    break;
                case "pip":
                    MenuFlyoutItem menuFlyoutItem = new()
                    {
                        Tag = this
                    };
                    CommonlyAccessedInstances.blankPage1.OpenPiP(menuFlyoutItem, null);
                    break;
                default:
                    CommonlyAccessedInstances.BasicGridView.Items.Remove(this);
                    CommonlyAccessedInstances.PinnedView.Items.Remove(this);
                    Thread thread = new(CommonlyAccessedInstances.blankPage1.SaveTo);
                    thread.Start();
                    break;
            }
        }

    }
}
