using CompanionDisplayWinUI.API;
using CompanionDisplayWinUI.ClassImplementations;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Net.Http;
using System.Threading;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UpdateWarning : Page
    {
        public UpdateWarning()
        {
            this.InitializeComponent();
            Thread thread0 = new(UpdateUI);
            thread0.Start();
        }
        private async void UpdateUI()
        {
            try{
                using HttpClient client = new();
                string version, reply;
                if (Globals.IsBetaProgram)
                {
                    reply = await client.GetStringAsync("https://github.com/kurakanai/Companion-Display/releases/download/prerelease_latest/changelog.txt");
                    version = await client.GetStringAsync(Globals.UpdateStringBeta);
                }
                else
                {
                    reply = await client.GetStringAsync("https://github.com/kurakanai/Companion-Display/releases/latest/download/changelog.txt");
                    version = await client.GetStringAsync(Globals.UpdateString);
                }
                DispatcherQueue.TryEnqueue(() =>
                {
                    VersionName.Text = AppStrings.UpdateVersion(version);
                    ChangelogBlock.Text = reply;
                });
            }
            catch{
                DispatcherQueue.TryEnqueue(() =>
                {
                    CommonlyAccessedInstances.BasicGridView.Items.Remove(this);
                });
            }
        }
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            UpdateBtn.IsEnabled = false;
            UpdateBtn.Content = AppStrings.updateUpdating;
            await MaintenanceAPI.PerformUpdateAsync(true);
        }
    }
}
