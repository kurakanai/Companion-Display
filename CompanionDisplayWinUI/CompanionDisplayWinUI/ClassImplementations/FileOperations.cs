using Microsoft.UI.Xaml.Controls;
using System;
using System.IO;

namespace CompanionDisplayWinUI.ClassImplementations
{
    class FileOperations
    {
        public static void DeleteFile(string filepath)
        {
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }
        }
        public static void SaveGridLayout(GridView gridView, string path)
        {
            var Items = gridView.Items;
            string Order = "";
            foreach (var widget in Items)
            {
                try
                {
                    var item = widget as Frame;
                    string deedify = item.Content.ToString().Replace("WidgetSettings", "");
                    if (!deedify.Contains("CompanionDisplayWinUI.UpdateWarning") && !deedify.Contains("Microsoft.UI.Xaml.Controls.Button"))
                    {
                        Order += deedify;
                        switch (deedify)
                        {
                            case var s when (deedify.Contains("CompanionDisplayWinUI.NotesWidget") || deedify.Contains("CompanionDisplayWinUI.WidgetStack")):
                                Order += "ID" + item.Name.ToString();
                                break;
                            case var s when deedify.Contains("CompanionDisplayWinUI.WidgetPhoto"):
                                Order += "IMAGESOURCE" + item.Tag.ToString();
                                break;
                        }
                        Order += Environment.NewLine;
                    }
                }
                catch
                {
                }
            }
            File.WriteAllText(path, Order);
        }
    }
}
