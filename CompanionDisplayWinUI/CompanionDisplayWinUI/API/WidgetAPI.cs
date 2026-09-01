using CompanionDisplayWinUI.ClassImplementations;
using CompanionDisplayWinUI.ClassImplementations.SharedPages;
using CompanionDisplayWinUI.Objects;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CompanionDisplayWinUI.API
{
    static class WidgetAPI
    {
        public static void addWidgets(Selector target, string configFile)
        {
            CommonlyAccessedInstances.mainDispatcher.TryEnqueue(() =>
            {
                StringReader stringReader = new StringReader(configFile);
                string buffer = stringReader.ReadLine();
                while (buffer != null)
                {
                    try
                    {
                        buffer = buffer.Replace("\r", "");
                        CommonWidgetContainer widgetToAdd = attributeApplier(patcher(buffer));
                        target.Items.Add(widgetToAdd);
                    }
                    catch { }
                    buffer = stringReader.ReadLine();
                }
            });
        }
        public static object[] patcher(string lineToParse)
        {
            object[] parameters = new object[3];
            switch (lineToParse)
            {
                case string a when a.Contains("CompanionDisplayWinUI.WidgetPhotoIMAGESOURCE"):
                    parameters[0] = "CompanionDisplayWinUI.WidgetPhoto";
                    parameters[1] = lineToParse.Replace("CompanionDisplayWinUI.WidgetPhotoIMAGESOURCE", "");
                    parameters[2] = false;
                    break;
                case string b when b.Contains("CompanionDisplayWinUI.NotesWidgetID"):
                    parameters[0] = "CompanionDisplayWinUI.NotesWidget";
                    parameters[1] = lineToParse.Replace("CompanionDisplayWinUI.NotesWidgetID", "");
                    parameters[2] = false;
                    break;
                case string c when c.Contains("CompanionDisplayWinUI.WidgetStackID"):
                    parameters[0] = "CompanionDisplayWinUI.WidgetStack";
                    parameters[1] = lineToParse.Replace("CompanionDisplayWinUI.WidgetStackID", "");
                    parameters[2] = true;
                    break;
                default:
                    parameters[0] = lineToParse;
                    parameters[2] = false;
                    break;
            }
            return parameters;
        }
        public static CommonWidgetContainer attributeApplier(object[] patched)
        {
            Type type = Type.GetType((string)patched[0]);
            CommonWidgetContainer commonWidget = new(type)
            {
                CornerRadius = new CornerRadius(8),
                Background = (AcrylicBrush)Application.Current.Resources["CustomAcrylicInAppLuminosity"],
            };
            if ((bool)patched[2])
            {
                commonWidget.Name = (string)patched[1];
            }
            else
            {
                commonWidget.Tag = patched[1];
            }
            return commonWidget;
        }

    }
}
