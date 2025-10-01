using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;

namespace CompanionDisplayWinUI.ClassImplementations
{
    internal class VariableQuirks
    {
        public static int getIntFromBool(bool value, int ifTrue, int ifFalse)
        {
            return value ? ifTrue : ifFalse;
        }
        public static Visibility getVisibilityFromBool(bool value)
        {
            return (Visibility)getIntFromBool(value, 0, 1);
        }
        public static AppWindowPresenterKind getPresenterKindFromBool(bool value)
        {
            return (AppWindowPresenterKind)getIntFromBool(value, 0, 2);
        }
    }
}
