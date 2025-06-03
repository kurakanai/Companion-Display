using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using System;
using System.IO;

namespace CompanionDisplayWinUI.ClassImplementations
{
    static class BackupOperations
    {
        public static event CommonlyAccessedInstances.HandleEventsWithNoArgs BackupFinished;
        private static string enableGeneralString = " del /f /q ..\\GlobalSettings.crlh & copy GlobalSettings.crlh .. &";
        private static string enableWidgetLayoutString = " del /f /q ..\\PinnedOrder.crlh & copy PinnedOrder.crlh .. & del /f /q ..\\WidgetOrder.crlh & copy WidgetOrder.crlh .. & rmdir /S /Q ..\\Stacks & robocopy Stacks ../Stacks /mir & rmdir /S /Q ..\\WidgetNotes & robocopy WidgetNotes ../WidgetNotes /mir &";
        private static string enableWidgetSettingsString = " del /f /q ..\\MacroThumbs.crlh & copy MacroThumbs.crlh .. & del /f /q ..\\MediaConfig.crlh & copy MediaConfig.crlh .. & del /f /q ..\\OBSSettings.crlh & copy OBSSettings.crlh .. & del /f /q ..\\PhotoConfig.crlh & copy PhotoConfig.crlh .. & del /f /q ..\\RefreshToken.crlh & copy RefreshToken.crlh .. & del /f /q ..\\RefreshToken2.crlh & copy RefreshToken2.crlh .. & del /f /q ..\\TimeConfigQS.crlh & copy TimeConfigQS.crlh .. &";
        private static string enableGeneralStringBackup = " copy GlobalSettings.crlh Backup &";
        private static string enableWidgetLayoutStringBackup = " copy PinnedOrder.crlh Backup & copy WidgetOrder.crlh Backup & robocopy Stacks Backup/Stacks /mir & robocopy WidgetNotes Backup/WidgetNotes /mir &";
        private static string enableWidgetSettingsStringBackup = " copy MacroThumbs.crlh Backup & copy MediaConfig.crlh Backup & copy OBSSettings.crlh Backup & copy PhotoConfig.crlh Backup & copy RefreshToken.crlh Backup & copy RefreshToken2.crlh Backup & copy TimeConfigQS.crlh Backup &";
        static void BackupFinishedMethod()
        {
            BackupFinished?.Invoke();
        }
        public static void RestoreBackup(bool enableGeneral, bool enableWidgetLayout, bool enableWidgetSettings, string backupFile)
        {
            CMDOperations.PerformCMDCommand("taskkill /f /im msedgewebview2.exe");
            string options = "&";
            if (enableGeneral)
            {
                options += enableGeneralString;
            }
            if (enableWidgetLayout)
            {
                options += enableWidgetLayoutString;
            }
            if (enableWidgetSettings)
            {
                options += enableWidgetSettingsString;
            }
            if (!Directory.Exists("Config"))
            {
                Directory.CreateDirectory("Config");
            }
            CMDOperations.PerformCMDCommand("cd Config & rmdir /S /Q Backup & mkdir Backup & cd Backup & tar -xf \"" + backupFile + "\" " + options + " cd .. & rmdir /S /Q Backup");
            Microsoft.Windows.AppLifecycle.AppInstance.Restart("");
        }
        public static void PerformBackup(bool enableGeneral, bool enableWidgetLayout, bool enableWidgetSettings, string backupFile)
        {
            string options = "&";
            if (enableGeneral)
            {
                options += enableGeneralStringBackup;
            }
            if (enableWidgetLayout)
            {
                options += enableWidgetLayoutStringBackup;
            }
            if (enableWidgetSettings)
            {
                options += enableWidgetSettingsStringBackup;
            }
            if (!Directory.Exists("Config"))
            {
                Directory.CreateDirectory("Config");
            }
            CMDOperations.PerformCMDCommand("cd Config & rmdir /S /Q Backup & mkdir Backup " + options + " cd Backup & tar -a -c -f " + backupFile + " * & cd .. & rmdir /S /Q Backup");
        }
        public async static void OpenDialog(XamlRoot xamlRoot, bool isBackup)
        {
            ContentDialog dialog = new()
            {
                XamlRoot = xamlRoot,
                Style = Microsoft.UI.Xaml.Application.Current.Resources["DefaultContentDialogStyle"] as Style
            };
            if (isBackup)
            {
                dialog.PrimaryButtonText = AppStrings.backupString;
                dialog.DefaultButton = ContentDialogButton.Primary;
                dialog.Title = AppStrings.backupTitleBar;
            }
            else
            {
                dialog.DefaultButton = ContentDialogButton.Secondary;
                dialog.Title = AppStrings.restoreHeaderString;
            }
            StackPanel stackPanel = new();
            CheckBox checkBox0 = new()
            {
                Content = AppStrings.configTypeGeneralSettings
            };
            CheckBox checkBox1 = new()
            {
                Content = AppStrings.configTypeWidgetLayout
            };
            CheckBox checkBox2 = new()
            {
                Content = AppStrings.configTypeWidgetSettings
            };
            stackPanel.Children.Add(checkBox0);
            stackPanel.Children.Add(checkBox1);
            stackPanel.Children.Add(checkBox2);
            dialog.Content = stackPanel;
            dialog.SecondaryButtonText = AppStrings.restoreString;
            dialog.CloseButtonText = AppStrings.cancelString;
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                string backuplocation = FileFolderPicker.SaveFileDialog(AppStrings.backupFilters, AppStrings.backupTitleBar);
                if (backuplocation != "")
                {
                    PerformBackup(checkBox0.IsChecked.Value, checkBox1.IsChecked.Value, checkBox2.IsChecked.Value, backuplocation);
                }
            }
            else if (result == ContentDialogResult.Secondary)
            {
                try
                {
                    dialog.SecondaryButtonText = AppStrings.restoringString;
                    string backuplocation = FileFolderPicker.OpenFileDialog(false)[0];
                    RestoreBackup(checkBox0.IsChecked.Value, checkBox1.IsChecked.Value, checkBox2.IsChecked.Value, backuplocation);
                }
                catch { }
            }
            BackupFinishedMethod();
        }
    }
}
