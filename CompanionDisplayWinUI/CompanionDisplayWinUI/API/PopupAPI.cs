using CompanionDisplayWinUI.ClassImplementations;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace CompanionDisplayWinUI.API
{
    static class PopupAPI
    {
        public async static void OpenSleepDialogue(XamlRoot xamlRoot)
        {
            ContentDialog dialog = new()
            {
                XamlRoot = xamlRoot,
                Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                DefaultButton = ContentDialogButton.Primary
            };
            if (Globals.sleepTimer.isEnabled)
            {
                dialog.Title = AppStrings.sleepTimerEnd;
                dialog.Content = AppStrings.sleepTimerAlreadyActive;
                dialog.PrimaryButtonText = AppStrings.sleepTimerEnd;
                dialog.CloseButtonText = AppStrings.cancelString;
                var result = await dialog.ShowAsync();
                if (result == ContentDialogResult.Primary && Globals.sleepTimer.isEnabled)
                {
                    Globals.sleepTimer.CancelTimer();
                }
            }
            else
            {
                dialog.Title = AppStrings.sleepTimer;
                dialog.PrimaryButtonText = AppStrings.sleepTimerStart;
                dialog.CloseButtonText = AppStrings.cancelString;
                NumberBox numberBox = new()
                {
                    PlaceholderText = AppStrings.sleepTimerMinsPlaceholder,
                    SpinButtonPlacementMode = NumberBoxSpinButtonPlacementMode.Inline,
                    SmallChange = 1,
                    LargeChange = 5,
                    Minimum = 1
                };
                dialog.Content = numberBox;
                var result = await dialog.ShowAsync();
                if (result == ContentDialogResult.Primary && !Globals.sleepTimer.isEnabled)
                {
                    Globals.sleepTimer.StartTimer((int)numberBox.Value);
                }
            }
        }
    }
}
