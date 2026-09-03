using CompanionDisplayWinUI.ClassImplementations;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.IO;

namespace CompanionDisplayWinUI.API
{
    static class BackupAPI
    {
        public static event CommonlyAccessedInstances.HandleEventsWithNoArgs BackupFinished;
        private static readonly string enableGeneralStringBackup = " copy GlobalSettings.crlh Backup &", enableWidgetLayoutStringBackup = " copy PinnedOrder.crlh Backup & copy WidgetOrder.crlh Backup & robocopy Stacks Backup/Stacks /mir & robocopy WidgetNotes Backup/WidgetNotes /mir &", enableWidgetSettingsStringBackup = " copy MacroThumbs.crlh Backup & copy MediaConfig.crlh Backup & copy OBSSettings.crlh Backup & copy PhotoConfig.crlh Backup & copy RefreshToken.crlh Backup & copy RefreshToken2.crlh Backup & copy TimeConfigQS.crlh Backup &";
        static void BackupFinishedMethod()
        {
            BackupFinished?.Invoke();
        }

        public static void RestoreBackup(bool enableGeneral, bool enableWidgetLayout, bool enableWidgetSettings, string backupFile)
        {
            FileAPI.ExtractFile(backupFile, "Config/Backup");
            if (enableGeneral){
                FileAPI.MoveToTopFileOvr("Config/Backup/GlobalSettings.crlh");
                FileAPI.MoveToTopFileOvr("Config/Backup/SecSettings.crlh");
            }
            if (enableWidgetLayout)
            {
                FileAPI.MoveToTopFileOvr("Config/Backup/PinnedOrder.crlh");
                FileAPI.MoveToTopFileOvr("Config/Backup/WidgetOrder.crlh");
                FileAPI.MoveToTopDirOvr("Config/Backup/Stacks");
                FileAPI.MoveToTopDirOvr("Config/Backup/WidgetNotes");
            }
            if (enableWidgetSettings)
            {
                FileAPI.MoveToTopFileOvr("Config/Backup/MacroThumbs.crlh");
                FileAPI.MoveToTopFileOvr("Config/Backup/MediaConfig.crlh");
                FileAPI.MoveToTopFileOvr("Config/Backup/OBSSettings.crlh");
                FileAPI.MoveToTopFileOvr("Config/Backup/PhotoConfig.crlh");
                FileAPI.MoveToTopFileOvr("Config/Backup/RefreshToken.crlh");
                FileAPI.MoveToTopFileOvr("Config/Backup/RefreshToken2.crlh");
                FileAPI.MoveToTopFileOvr("Config/Backup/TimeConfigQS.crlh");
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
            CommandAPI.PerformCMDCommand("cd Config & rmdir /S /Q Backup & mkdir Backup " + options + " cd Backup & tar -a -c -f " + backupFile + " * & cd .. & rmdir /S /Q Backup");
        }
        public static void EraseConfig(){
            CommandAPI.PerformCMDCommand("rmdir /S /Q Config");
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
                string backuplocation = FileAPI.SaveFileDialog(AppStrings.backupFilters, AppStrings.backupTitleBar);
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
                    string backuplocation = FileAPI.OpenFileDialog(false)[0];
                    RestoreBackup(checkBox0.IsChecked.Value, checkBox1.IsChecked.Value, checkBox2.IsChecked.Value, backuplocation);
                }
                catch { }
            }
            BackupFinishedMethod();
        }
    }
}
