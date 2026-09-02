using System.Security.Principal;

namespace WinFormsHelper
{
    public class Class1
    {
        public static bool IsAdmin()
        {
            using WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
