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
            using HttpClient client = new();
            string version, reply;
            if (Globals.IsBetaProgram)
            {
                reply = await client.GetStringAsync("https://www.dropbox.com/scl/fi/hr6tiqeazmkzllsz9y53z/changelog.txt?rlkey=s40umhmz174z7tok6guelylnu&st=a68s40wb&dl=1");
                version = await client.GetStringAsync(Globals.UpdateStringBeta);
            }
            else
            {
                reply = await client.GetStringAsync("https://www.dropbox.com/scl/fi/f8nu0pn6leiz646o1ebce/changelog.txt?rlkey=fd6srkx7xm3wexk1eevglgs8c&st=mtnphh0r&dl=1");
                version = await client.GetStringAsync(Globals.UpdateString);
            }
            DispatcherQueue.TryEnqueue(() =>
            {
                VersionName.Text = AppStrings.UpdateVersion(version);
                ChangelogBlock.Text = reply;
            });
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            using HttpClient client = new();
            UpdateBtn.IsEnabled = false;
            UpdateBtn.Content = AppStrings.updateUpdating;
            UpdateSystem.PerformUpdate(true);
        }
    }
}
