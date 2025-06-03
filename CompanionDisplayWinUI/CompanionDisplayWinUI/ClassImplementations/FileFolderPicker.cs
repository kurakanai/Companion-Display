using System.Windows.Forms;

namespace CompanionDisplayWinUI.ClassImplementations
{
    static class FileFolderPicker
    {
        public static string[] OpenFileDialog(bool multiselect)
        {
            using OpenFileDialog openFileDialog1 = new() { DereferenceLinks = false, Multiselect = multiselect, InitialDirectory = "%AppData%\\Microsoft\\Windows\\Start Menu\\Programs", FilterIndex = 0, RestoreDirectory = true };
            openFileDialog1.ShowDialog();
            return openFileDialog1.FileNames;
        }
        public static string OpenFolder()
        {
            using FolderBrowserDialog openFileDialog1 = new() { InitialDirectory = "%AppData%\\Microsoft\\Windows\\Start Menu\\Programs" };
            openFileDialog1.ShowDialog();
            return openFileDialog1.SelectedPath;
        }
        public static string SaveFileDialog(string Filter, string Title)
        {
            using SaveFileDialog saveFileDialog = new() { Filter = Filter, Title = Title };
            saveFileDialog.ShowDialog();
            return saveFileDialog.FileName;
        }
    }
}
