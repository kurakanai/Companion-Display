using CompanionDisplayWinUI.ClassImplementations;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace CompanionDisplayWinUI.API
{
    class MaintenanceAPI
    {
        public async static void PerformUpdate(bool downloadUpdate)
        {
            string updatePath = "";
            if (downloadUpdate)
            {
                updatePath = "release.zip";
                string chosenPath = Globals.UpdateZip;
                if (Globals.IsBetaProgram)
                {
                    chosenPath = Globals.UpdateZipBeta;
                }
                using var s = await CommonlyAccessedInstances.client.GetStreamAsync(chosenPath);
                using var fs = new FileStream("release.zip", FileMode.CreateNew);
                await s.CopyToAsync(fs);
            }
            else
            {
                try
                {
                    updatePath = FileAPI.OpenFileDialog(false)[0];
                }
                catch { }
            }
            if(updatePath != "")
            {
                CommandAPI.PerformCMDCommand("taskkill /f /im CompanionDisplayWinUI.exe & mkdir Update & MOVE * Update/ & cd Update & move CompanionDisplayWinUI.exe.WebView2 ../ & move Config ../ & move \"" + updatePath + "\" ../release.zip & move setup.exe ../ & cd .. & tar -xf release.zip & del /f /q release.zip & rmdir /s /q Update & CompanionDisplayWinUI.exe");
            }
        }
        public static async Task CheckUpdate()
        {
            using HttpClient client = new();
            string reply = await client.GetStringAsync(Globals.UpdateString);
            if (Globals.IsBetaProgram)
            {
                reply = await client.GetStringAsync(Globals.UpdateStringBeta);
            }
            Globals.IsUpdateAvailable = !(reply == Globals.Version);
        }
    }
}
