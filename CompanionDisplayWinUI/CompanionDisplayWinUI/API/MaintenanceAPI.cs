using CompanionDisplayWinUI.ClassImplementations;
using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace CompanionDisplayWinUI.API
{
    class MaintenanceAPI
    {
        public static async Task PerformUpdateAsync(bool downloadUpdate)
        {
            string appDir = AppContext.BaseDirectory;
            string stagingDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "CompanionDisplay",
                "UpdateStaging_" + Guid.NewGuid().ToString("N")
            );

            Directory.CreateDirectory(stagingDir);
            string stagedZipPath = Path.Combine(stagingDir, "release.zip");
            string stagedSigPath = Path.Combine(stagingDir, "release.zip.sig");
            try
            {
                if (downloadUpdate)
                {
                    string zipUrl = Globals.IsBetaProgram ? Globals.UpdateZipBeta : Globals.UpdateZip;
                    string sigUrl = Globals.IsBetaProgram ? Globals.UpdateZipSigBeta : Globals.UpdateZipSig;

                    // 1. Download both the archive and detached signature into the restricted staging folder
                    await DownloadFileAsync(zipUrl, stagedZipPath);
                    await DownloadFileAsync(sigUrl, stagedSigPath);
                    if (!VerifySignature(stagedZipPath, stagedSigPath))
                    {
                        throw new UnauthorizedAccessException("Update signature verification failed. Aborting update.");
                    }
                }
                else
                {
                    string[] selectedFiles;
                    try
                    {
                        selectedFiles = FileAPI.OpenFileDialog(false);
                    }
                    catch
                    {
                        return;
                    }

                    if (selectedFiles == null || selectedFiles.Length == 0 || string.IsNullOrWhiteSpace(selectedFiles[0]))
                        return;

                    // Copy user-selected local zip into staging (avoids CMD command injection via filename)
                    File.Copy(selectedFiles[0], stagedZipPath, overwrite: true);
                }
                string targetZipPath = Path.Combine(appDir, "release.zip");
                File.Move(stagedZipPath, targetZipPath, overwrite: true);
                RunSwapAndRestart(appDir);
            }
            finally
            {
                // Always clean up staging artifacts
                try
                {
                    if (Directory.Exists(stagingDir))
                        Directory.Delete(stagingDir, recursive: true);
                }
                catch { }
            }
        }

        private static async Task DownloadFileAsync(string url, string destinationPath)
        {
            using var response = await CommonlyAccessedInstances.client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            await using var fs = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None);
            await response.Content.CopyToAsync(fs);
        }

        private static bool VerifySignature(string zipPath, string sigPath)
        {
            if (!File.Exists(zipPath) || !File.Exists(sigPath))
                return false;

            using var ecdsa = ECDsa.Create();
            ecdsa.ImportFromPem(Globals.PublicKeyPem);

            byte[] zipBytes = File.ReadAllBytes(zipPath);
            byte[] sigBytes = File.ReadAllBytes(sigPath);

            return ecdsa.VerifyData(
                zipBytes,
                sigBytes,
                HashAlgorithmName.SHA256,
                DSASignatureFormat.Rfc3279DerSequence
            );
        }

        private static void RunSwapAndRestart(string workingDirectory)
        {
            const string swapCommand =
                "taskkill /f /im CompanionDisplayWinUI.exe & " +
                "mkdir Update & " +
                "move * Update\\ & " +
                "cd Update & " +
                "move CompanionDisplayWinUI.exe.WebView2 ..\\ & " +
                "move Config ..\\ & " +
                "move release.zip ..\\ & " +
                "move setup.exe ..\\ & " +
                "cd .. & " +
                "tar -xf release.zip & " +
                "del /f /q release.zip & " +
                "rmdir /s /q Update & " +
                "start \"\" CompanionDisplayWinUI.exe";
            CommandAPI.PerformCMDCommand(swapCommand);
        }
        //public async static void PerformUpdate(bool downloadUpdate)
        //{
        //    string updatePath = string.Empty;
        //    if (downloadUpdate)
        //    {
        //        updatePath = "release.zip";
        //        string chosenPath = Globals.UpdateZip;
        //        if (Globals.IsBetaProgram)
        //        {
        //            chosenPath = Globals.UpdateZipBeta;
        //        }
        //        using var s = await CommonlyAccessedInstances.client.GetStreamAsync(chosenPath);
        //        using var fs = new FileStream("release.zip", FileMode.CreateNew);
        //        await s.CopyToAsync(fs);
        //    }
        //    else
        //    {
        //        try
        //        {
        //            updatePath = FileAPI.OpenFileDialog(false)[0];
        //        }
        //        catch { }
        //    }
        //    if(updatePath != "")
        //    {
        //        CommandAPI.PerformCMDCommand($"taskkill /f /im CompanionDisplayWinUI.exe & mkdir Update & MOVE * Update/ & cd Update & move CompanionDisplayWinUI.exe.WebView2 ../ & move Config ../ & move \"{updatePath}\" ../release.zip & move setup.exe ../ & cd .. & tar -xf release.zip & del /f /q release.zip & rmdir /s /q Update & CompanionDisplayWinUI.exe");
        //    }
        //}
        public static async Task CheckUpdate()
        {
            using HttpClient client = new();
            string reply = Globals.IsBetaProgram ? await client.GetStringAsync(Globals.UpdateStringBeta) : await client.GetStringAsync(Globals.UpdateString);
            Globals.IsUpdateAvailable = !(reply == Globals.Version);
        }
    }
}
