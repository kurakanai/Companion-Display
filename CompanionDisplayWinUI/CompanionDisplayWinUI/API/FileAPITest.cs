using System;
using System.Collections.Generic;
using System.Formats.Tar;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanionDisplayWinUI.API
{
    static class FileAPITest
    {
        public static void ExtractFile(string file, string destination)
        {
            try
            {
                Directory.CreateDirectory(destination);
                TarFile.ExtractToDirectory(file, destination, true);
            }
            catch { }
        }
        private static void MoveFile(string file, string destination, bool ovr)
        {
            try
            {
                File.Move(file, destination, ovr);
            }
            catch { }
        }
        public static void MoveFileSafe(string file, string destination)
        {
            MoveFile(file, destination, false);
        }
        public static void MoveFileOverwrite(string file, string destination)
        {
            MoveFile(file, destination, true);
        }
        private static void MoveDir(string source, string destination, bool ovr)
        {
            if (ovr)
            {
                try
                {
                    Directory.Delete(destination, true);
                }
                catch { }
            }
            Directory.Move(source, destination);
        }
        public static void MoveDirSafe(string source, string destination)
        {
            
        }
    }
}
