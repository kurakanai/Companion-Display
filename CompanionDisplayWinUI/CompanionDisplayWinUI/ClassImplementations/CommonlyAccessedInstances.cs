using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Net.Http;

namespace CompanionDisplayWinUI.ClassImplementations
{
    class CommonlyAccessedInstances
    {
        public delegate void HandleEventsWithNoArgs();
        internal static Window m_window;
        public static NavigationView nvSample;
        public static GridView BasicGridView;
        public static GridView PinnedView;
        public static HttpClient client = new();
    }
}
