using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Shapes;
using System.Net.Http;
using System.Windows.Threading;

namespace CompanionDisplayWinUI.ClassImplementations
{
    class CommonlyAccessedInstances
    {
        public delegate void HandleEventsWithNoArgs();
        internal static Window m_window;
        public static NavigationView nvSample;
        public static GridView BasicGridView;
        public static GridView PinnedView;
        public static Grid ScalingGrid;
        public static HttpClient client = new();
        public static Grid MainGrid;
        public static Rectangle WindowControls;
        public static DispatcherQueue mainDispatcher;
        // very messy but needed to ship 26.2 in time
        public static BlankPage1 blankPage1;
    }
}
