using System.Diagnostics;

namespace CompanionDisplayWinUI.ClassImplementations
{
    static class CMDOperations
    {
        private static readonly Process cmd = new()
        {
            StartInfo = new()
            {
                FileName = "cmd.exe",
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                RedirectStandardOutput = true
            }
        };
        private static readonly Process ps = new()
        {
            StartInfo = new()
            {
                FileName = "powershell.exe",
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                RedirectStandardOutput = true
            }
        };
        public static void PerformCMDCommand(string command)
        {
            cmd.StartInfo.Arguments = "/C " + command;
            cmd.Start();
            cmd.WaitForExit();
        }
        public static void PerformPowershellCommand(string command)
        {
            ps.StartInfo.Arguments = command;
            ps.Start();
            ps.WaitForExit();
        }
        public static string GetCMDLog(string command)
        {
            PerformCMDCommand(command);
            return cmd.StandardOutput.ReadToEnd();
        }
        public static string GetPowershellLog(string command)
        {
            PerformPowershellCommand(command);
            return ps.StandardOutput.ReadToEnd();
        }
    }
}
