using Microsoft.Graphics.Canvas.Effects;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Hosting;

namespace CompanionDisplayWinUI.ClassImplementations
{
    public static class FancyPantsUX
    {
        public static void SetupMaskedContainer(FrameworkElement host, FrameworkElement content)
        {
            var compositor = ElementCompositionPreview.GetElementVisual(host).Compositor;
            var alphaEffect = new AlphaMaskEffect
            {
                Source = new CompositionEffectSourceParameter("source"),
                AlphaMask = new CompositionEffectSourceParameter("mask")
            };
            var effectFactory = compositor.CreateEffectFactory(alphaEffect);
            var effectBrush = effectFactory.CreateBrush();
            var maskBrush = compositor.CreateLinearGradientBrush();
            maskBrush.StartPoint = new System.Numerics.Vector2(0, 0);
            maskBrush.EndPoint = new System.Numerics.Vector2(1, 0);
            maskBrush.ColorStops.Add(compositor.CreateColorGradientStop(0.0f, Microsoft.UI.Colors.Transparent));
            maskBrush.ColorStops.Add(compositor.CreateColorGradientStop(0.02f, Microsoft.UI.Colors.White));
            maskBrush.ColorStops.Add(compositor.CreateColorGradientStop(0.98f, Microsoft.UI.Colors.White));
            maskBrush.ColorStops.Add(compositor.CreateColorGradientStop(1.0f, Microsoft.UI.Colors.Transparent));
            var contentVisual = ElementCompositionPreview.GetElementVisual(content);
            var surface = compositor.CreateVisualSurface();
            surface.SourceVisual = contentVisual;
            surface.SourceSize = new System.Numerics.Vector2((float)host.ActualWidth, (float)host.ActualHeight);
            var surfaceBrush = compositor.CreateSurfaceBrush();
            surfaceBrush.Surface = surface;
            effectBrush.SetSourceParameter("source", surfaceBrush);
            effectBrush.SetSourceParameter("mask", maskBrush);
            var sprite = compositor.CreateSpriteVisual();
            sprite.Brush = effectBrush;
            sprite.Size = new System.Numerics.Vector2((float)host.ActualWidth, (float)host.ActualHeight);
            ElementCompositionPreview.SetElementChildVisual(host, sprite);
            host.SizeChanged += (s, e) =>
            {
                sprite.Size = new System.Numerics.Vector2((float)host.ActualWidth, (float)host.ActualHeight);
                surface.SourceSize = new System.Numerics.Vector2((float)host.ActualWidth, (float)host.ActualHeight);
            };
        }

    }
}
