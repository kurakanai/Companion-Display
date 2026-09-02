using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;

namespace CompanionDisplayWinUI.ClassImplementations
{
    internal class VariableQuirks
    {
        public static int GetIntFromBool(bool value, int ifTrue, int ifFalse)
        {
            return value ? ifTrue : ifFalse;
        }
        public static Visibility GetVisibilityFromBool(bool value)
        {
            return (Visibility)GetIntFromBool(value, 0, 1);
        }
        public static AppWindowPresenterKind GetPresenterKindFromBool(bool value)
        {
            return (AppWindowPresenterKind)GetIntFromBool(value, 0, 2);
        }
    }
}
