using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using System.IO;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NotesWidget : Page
    {
        public NotesWidget()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }
        public string GetNote()
        {
            return Notes.Text.Replace("\n", "NOTENEWLINE");
        }
        private bool FTU = true;
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (FTU)
            {
                FTU = !FTU;
                try
                {
                    Notes.Text = File.ReadAllText("Config/WidgetNotes/" + (this.Parent as Frame).Tag.ToString() + ".crlh");
                }
                catch { }
            }
        }
        private void Notes_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                Directory.CreateDirectory("Config/WidgetNotes");
                File.WriteAllText("Config/WidgetNotes/" + (this.Parent as Frame).Tag.ToString() + ".crlh", Notes.Text);
            }
            catch { }
        }

        private void Notes_Tapped(object sender, TappedRoutedEventArgs e)
        {
        }
    }
}
