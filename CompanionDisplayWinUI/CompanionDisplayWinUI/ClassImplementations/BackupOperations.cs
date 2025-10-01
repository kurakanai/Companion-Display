using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.IO;

namespace CompanionDisplayWinUI.ClassImplementations
{
    static class BackupOperations
    {
        public static event CommonlyAccessedInstances.HandleEventsWithNoArgs BackupFinished;
        private static string enableGeneralStringBackup = " copy GlobalSettings.crlh Backup &";
        private static string enableWidgetLayoutStringBackup = " copy PinnedOrder.crlh Backup & copy WidgetOrder.crlh Backup & robocopy Stacks Backup/Stacks /mir & robocopy WidgetNotes Backup/WidgetNotes /mir &";
        private static string enableWidgetSettingsStringBackup = " copy MacroThumbs.crlh Backup & copy MediaConfig.crlh Backup & copy OBSSettings.crlh Backup & copy PhotoConfig.crlh Backup & copy RefreshToken.crlh Backup & copy RefreshToken2.crlh Backup & copy TimeConfigQS.crlh Backup &";
        static void BackupFinishedMethod()
        {
            BackupFinished?.Invoke();
        }

        public static void RestoreBackup(bool enableGeneral, bool enableWidgetLayout, bool enableWidgetSettings, string backupFile)
        {
            FileOps.ExtractFile(backupFile, "Config/Backup");
            if (enableGeneral){
                FileOps.MoveToTopFileOvr("Config/Backup/GlobalSettings.crlh");
                FileOps.MoveToTopFileOvr("Config/Backup/SecSettings.crlh");
            }
            if (enableWidgetLayout)
            {
                FileOps.MoveToTopFileOvr("Config/Backup/PinnedOrder.crlh");
                FileOps.MoveToTopFileOvr("Config/Backup/WidgetOrder.crlh");
                FileOps.MoveToTopDirOvr("Config/Backup/Stacks");
                FileOps.MoveToTopDirOvr("Config/Backup/WidgetNotes");
            }
            if (enableWidgetSettings)
            {
                FileOps.MoveToTopFileOvr("Config/Backup/MacroThumbs.crlh");
                FileOps.MoveToTopFileOvr("Config/Backup/MediaConfig.crlh");
                FileOps.MoveToTopFileOvr("Config/Backup/OBSSettings.crlh");
                FileOps.MoveToTopFileOvr("Config/Backup/PhotoConfig.crlh");
                FileOps.MoveToTopFileOvr("Config/Backup/RefreshToken.crlh");
                FileOps.MoveToTopFileOvr("Config/Backup/RefreshToken2.crlh");
                FileOps.MoveToTopFileOvr("Config/Backup/TimeConfigQS.crlh");
            }
            Directory.Delete("Config/Backup", true);
            Microsoft.Windows.AppLifecycle.AppInstance.Restart("Backup Restore in progress...");
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
                Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style
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
