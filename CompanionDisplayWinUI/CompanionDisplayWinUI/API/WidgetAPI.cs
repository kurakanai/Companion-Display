using CompanionDisplayWinUI.ClassImplementations;
using CompanionDisplayWinUI.Objects;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using System;
using System.IO;

namespace CompanionDisplayWinUI.API
{
    static class WidgetAPI
    {
        public static void AddWidgets(Selector target, string configFile)
        {
            CommonlyAccessedInstances.mainDispatcher.TryEnqueue(() =>
            {
                StringReader stringReader = new(configFile);
                string buffer = stringReader.ReadLine();
                while (buffer != null)
                {
                    try
                    {
                        buffer = buffer.Replace("\r", "");
                        CommonWidgetContainer widgetToAdd = AttributeApplier(Patcher(buffer));
                        target.Items.Add(widgetToAdd);
                    }
                    catch { }
                    buffer = stringReader.ReadLine();
                }
            });
        }
        public static object[] Patcher(string lineToParse)
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
        public static CommonWidgetContainer AttributeApplier(object[] patched)
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
