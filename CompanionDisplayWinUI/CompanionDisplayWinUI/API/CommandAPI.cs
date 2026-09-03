using System.Diagnostics;

namespace CompanionDisplayWinUI.API
{
    static class CommandAPI
    {
        private static ProcessStartInfo GenerateProcessStartInfo(string executable)
        {
            ProcessStartInfo processStartInfo = new()
            {
                FileName = executable,
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                RedirectStandardOutput = true
            };
            return processStartInfo;
        }
        private static readonly Process cmd = new()
        {
            StartInfo = GenerateProcessStartInfo("cmd.exe"),
        };
        private static readonly Process ps = new()
        {
            StartInfo = GenerateProcessStartInfo("powershell.exe"),
        };
        public static void PerformCMDCommand(string command)
        {
            cmd.StartInfo.Arguments = $"/C {command}";
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
