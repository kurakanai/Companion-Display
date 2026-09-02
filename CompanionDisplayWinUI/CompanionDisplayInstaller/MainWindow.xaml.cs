using IWshRuntimeLibrary;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CompanionDisplayInstaller;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public string CurrentVersion = "25.4";
    public string RemoteVersion = "https://www.dropbox.com/scl/fi/bao8ksdeznvioqjp54yrh/releaseInstaller.txt?rlkey=m33mazijaavp0rgafzlkr68t1&st=fl4v7pp9&dl=1";
    public string InstallPath = "";
    public MainWindow()
    {
        InitializeComponent();
        if(Environment.OSVersion.Version.ToString().StartsWith("10.0.1") || Environment.OSVersion.Version.ToString().StartsWith("10.0.20"))
        {
            WindowsUnsupportedWarning.Visibility = Visibility.Collapsed;
        }
        else if (!Environment.OSVersion.Version.ToString().StartsWith("10"))
        {
            Windows10Warning.Visibility = Visibility.Collapsed;
        }
        else
        {
            WindowsUnsupportedWarning.Visibility = Visibility.Collapsed;
            Windows10Warning.Visibility = Visibility.Collapsed;
        }
        bool isElevated;
        using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
        {
            WindowsPrincipal principal = new(identity);
            isElevated = principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
        if (isElevated == true)
        {
            AdminWarning.Visibility = Visibility.Collapsed;
        }
        else
        {
            InstallOptions.Items.Remove(ProgramFilesOption);
        }
        string uninstallRegPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\CompanionDisplay";
        using RegistryKey? key = Registry.LocalMachine.OpenSubKey(uninstallRegPath);
        if (key != null)
        {
            InstallPath = key.GetValue("InstallLocation") as string;
            Intro.Text = "Companion Display is already installed. Here you can repair your install, which will also clear your configuration files, or you can uninstall Companion Display.";
            InstallOptions.Visibility = Visibility.Collapsed;
            InstallBtn.Content = "Repair";
        }
        else
        {
            UninstallBtn.Visibility = Visibility.Collapsed;
        }
    }

    private async void Button_Click(object sender, RoutedEventArgs e)
    {
        string installDir = "";
        if(InstallOptions.SelectedItem == ProgramFilesOption)
        {
            installDir = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\CompanionDisplay\\";
        }
        else if (InstallOptions.SelectedItem == AppDataOption)
        {
            installDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\CompanionDisplay\\";
        }
        else if(InstallOptions.SelectedItem == CustomDirOption)
        {
            installDir = CustDir.Text + "\\CompanionDisplay\\";
        }
        if(InstallPath != "")
        {
            installDir = InstallPath + "\\";
        }
        if(installDir != "")
        {
            using (HttpClient client = new())
            {
                InstallBtn.IsEnabled = false;
                InstallBtn.Content = "Installing...";
                using var s = await client.GetStreamAsync("https://www.dropbox.com/scl/fi/3pmay1a18v9wdi6y9ridh/release.zip?rlkey=9sbehxn68p4i18geb5acbe6y3&dl=1");
                using var s2 = await client.GetStreamAsync("https://www.dropbox.com/scl/fi/txml7xdncejg63pq5b80v/CompanionDisplayInstaller.exe?rlkey=4afy2supy5gj6iry1gw0r2rpq&st=9se29hw1&dl=1");
                using var s3 = await client.GetStreamAsync("https://aka.ms/SegoeFluentIcons");
                System.IO.Directory.CreateDirectory(installDir);
                using var fs = new FileStream(installDir + "release.zip", FileMode.Create);
                using var fs2 = new FileStream(installDir + "setup.exe", FileMode.Create);
                using var fs3 = new FileStream(installDir + "Segoe-Fluent-Icons.zip", FileMode.Create);
                await s.CopyToAsync(fs);
                await s2.CopyToAsync(fs2);
                string w10 = " & tar -xf Segoe-Fluent-Icons.zip & copy \"Segoe Fluent Icons.ttf\" \"%WINDIR%\\Fonts\" & reg add \"HKLM\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Fonts\" /v \"Segoe Fluent Icons (TrueType)\" /t REG_SZ /d \"Segoe Fluent Icons.ttf\" /f &  & del /f /q \"Segoe Fluent Icons.ttf\" & del /f /q Segoe-Fluent-Icons.zip & del /f /q EULA.txt";
                using Process cmd = new();
                cmd.StartInfo.FileName = "cmd.exe";
                cmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.Arguments = "/C cd \"" + installDir + "\" & tar -xf release.zip & del /f /q release.zip";
                if (Windows10Warning.Visibility == Visibility.Visible)
                {
                    await s2.CopyToAsync(fs2);
                    cmd.StartInfo.Arguments += w10;
                }
                cmd.Start();
                cmd.WaitForExit();
            }
            string uninstallRegPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\CompanionDisplay";
            RegistryKey key = Registry.LocalMachine.CreateSubKey(uninstallRegPath);
            key.SetValue("DisplayName", "Companion Display");
            key.SetValue("UninstallString", installDir + "setup.exe");
            key.SetValue("InstallLocation", installDir);
            key.SetValue("Publisher", "kurakanai");
            key.Close();
            string shortcutPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonPrograms), "Companion Display.lnk");
            string shortcutPathDesktop = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Companion Display.lnk");
            WshShell shell = new();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);
            IWshShortcut shortcutDesktop = (IWshShortcut)shell.CreateShortcut(shortcutPathDesktop);
            shortcut.TargetPath = installDir + "CompanionDisplayWinUI.exe";
            shortcut.WorkingDirectory = installDir;
            shortcutDesktop.TargetPath = shortcut.TargetPath;
            shortcutDesktop.TargetPath = shortcut.TargetPath;
            shortcut.Save();
            if (InstallOptions.SelectedItem == ProgramFilesOption)
            {
                SetShortcutToRunAsAdmin(shortcutPath);
            }
            if (CreateShort.IsChecked == true)
            {
                shortcutDesktop.Save();
                if (InstallOptions.SelectedItem == ProgramFilesOption)
                {
                    SetShortcutToRunAsAdmin(shortcutPathDesktop);
                }
            }
            if(LaunchAfterInstall.IsChecked == true)
            {
                using Process cmd = new();
                cmd.StartInfo.FileName = "cmd.exe";
                cmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.Arguments = "/C cd \"" + installDir + "\" & CompanionDisplayWinUI.exe\"";
                cmd.Start();
            }
            this.Close();
        }
    }

    private void Button_Click_1(object sender, RoutedEventArgs e)
    {
        var folderDialog = new OpenFolderDialog
        {
            
        };

        if (folderDialog.ShowDialog() == true)
        {
            CustDir.Text = folderDialog.FolderName;
        }
    }
    static void SetShortcutToRunAsAdmin(string shortcutPath)
    {
        FileInfo lnkFile = new(shortcutPath);
        System.IO.File.SetAttributes(lnkFile.FullName, FileAttributes.Normal);

        // Open shortcut file for modification
        using FileStream fs = new(lnkFile.FullName, FileMode.Open, FileAccess.ReadWrite);
        byte[] data = new byte[fs.Length];
        fs.ReadExactly(data);

        // Enable "Run as Administrator" (0x20 = RunAs flag)
        data[0x15] |= 0x20;

        fs.Position = 0;
        fs.Write(data, 0, data.Length);
    }
    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        using HttpClient client = new();
        if (CurrentVersion != await client.GetStringAsync(RemoteVersion))
        {
            UpdateInstaller.Visibility = Visibility.Visible;
        }
    }

    private void UninstallBtn_Click(object sender, RoutedEventArgs e)
    {
        string uninstallRegPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\CompanionDisplay";

        using (RegistryKey? key = Registry.LocalMachine.OpenSubKey(uninstallRegPath))
        {
            if (key != null)
            {
                string? installPath = key.GetValue("InstallLocation") as string;
                if (!string.IsNullOrEmpty(installPath))
                {
                    try
                    {
                        foreach (var process in Process.GetProcessesByName("CompanionDisplayWinUI"))
                        {
                            process.Kill();
                        }
                        
                        Registry.LocalMachine.DeleteSubKeyTree(uninstallRegPath);
                        string startMenuPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonPrograms), "Companion Display.lnk");
                        string desktopPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory), "Companion Display.lnk");
                        if (System.IO.File.Exists(startMenuPath)) System.IO.File.Delete(startMenuPath);
                        if (System.IO.File.Exists(desktopPath)) System.IO.File.Delete(desktopPath);
                        Directory.Delete(installPath, true);
                    }
                    catch
                    {
                    }
                }
            }
        }
        this.Close();
    }

    private async void UpdateInstaller_Click(object sender, RoutedEventArgs e)
    {
        using Process cmd = new();
        cmd.StartInfo.FileName = "cmd.exe";
        cmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        cmd.StartInfo.CreateNoWindow = true;
        cmd.StartInfo.Arguments = "/C start https://www.dropbox.com/scl/fi/txml7xdncejg63pq5b80v/CompanionDisplayInstaller.exe?rlkey=4afy2supy5gj6iry1gw0r2rpq&st=9se29hw1&dl=1";
        cmd.Start();
    }
}