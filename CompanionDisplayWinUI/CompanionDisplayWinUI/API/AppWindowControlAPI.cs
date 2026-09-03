using CompanionDisplayWinUI.ClassImplementations;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CompanionDisplayWinUI.API
{
    class AppWindowControlAPI
    {
        public static void RemoveWidget(Frame currentWidget)
        {
            GridView gridView = currentWidget.Parent as GridView;
            gridView.Items.Remove(currentWidget);
        }
        public static void SetScale(float scale, Visual rootVisual)
        {
            CommonlyAccessedInstances.MainGrid.HorizontalAlignment = HorizontalAlignment.Left;
            CommonlyAccessedInstances.MainGrid.VerticalAlignment = VerticalAlignment.Top;
            rootVisual.Scale = new System.Numerics.Vector3(scale, scale, 1.0f);
            UpdateScalingNoArgs();
        }
        public static void UpdateScaling()
        {
            if(Globals.scale != 1.0f)
            {
                CommonlyAccessedInstances.WindowControls.Visibility = Visibility.Collapsed;
            }
            else
            {
                CommonlyAccessedInstances.WindowControls.Visibility = Visibility.Visible;
            }
            CommonlyAccessedInstances.MainGrid.Width = CommonlyAccessedInstances.ScalingGrid.ActualWidth / Globals.scale;
            CommonlyAccessedInstances.MainGrid.Height = CommonlyAccessedInstances.ScalingGrid.ActualHeight / Globals.scale;
        }

        internal static void UpdateScalingNoArgs()
        {
            UpdateScaling();
        }
    }
}
