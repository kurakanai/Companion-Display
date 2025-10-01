using CompanionDisplayWinUI.ClassImplementations;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using System;
using System.Threading;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            System.Runtime.GCSettings.LargeObjectHeapCompactionMode = System.Runtime.GCLargeObjectHeapCompactionMode.CompactOnce;
            WindowsStuff.SetAdmin();
            ConfigurationOperations.LoadGeneralConfigs();
            this.InitializeComponent();
            Thread thread = new(InitializeOBS);
            thread.Start();
            Thread thread1 = new(InitMedia);
            thread1.Start();
            ConfigurationOperations.LoadSecConfig(DispatcherQueue.GetForCurrentThread());
        }

        private void InitializeOBS()
        {
            ConfigurationOperations.LoadOBSConfig();
            Globals.obsControls.Connect();
        }
        private void InitMedia()
        {
            Globals.StartedPlayer = true;
            Globals.playerSpotify.Page_Loaded();
        }
        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            CommonlyAccessedInstances.m_window = new MainWindow();
            CommonlyAccessedInstances.m_window.Activate();
            try
            {
                switch (Globals.ColorSchemeSelect)
                {
                    case (0):
                        break;
                    case (1):
                        ThemingAndColors.SetAppTheme(ElementTheme.Dark);
                        break;
                    case (2):
                        ThemingAndColors.SetAppTheme(ElementTheme.Light);
                        break;
                }
                ThemingAndColors.OverrideAccent();
                if (Globals.FontFamily != "")
                {
                    ThemingAndColors.SetFont(new Microsoft.UI.Xaml.Media.FontFamily(Globals.FontFamily));
                }
            }
            catch
            {
            }
        }
    }
}
