using System;
using System.Formats.Tar;
using System.IO;
using System.IO.Compression;

namespace CompanionDisplayWinUI.ClassImplementations
{
    internal class FileOps
    {
        public static void ExtractFile(string file, string destinationpath)
        {
            try
            {
                Directory.CreateDirectory(destinationpath);
                TarFile.ExtractToDirectory(file, destinationpath, true);
            }
            catch { }
        }
        public static void MoveToTopFileOvr(string file)
        {
            string parentDir = Directory.GetParent(file).FullName;
            string parentParentDir = Directory.GetParent(parentDir).FullName;
            MoveOverwrite(file, Path.Combine(parentParentDir, Path.GetFileName(file)));
        }
        public static void MoveToCurFileOvr(string file)
        {
            MoveOverwrite(file, Directory.GetCurrentDirectory());
        }
        public static void MoveToTopDirOvr(string folder)
        {
            string parentDir = Directory.GetParent(folder).FullName;
            string parentParentDir = Directory.GetParent(parentDir).FullName;
            MoveDirectoryOverwrite(folder, Path.Combine(parentParentDir, Path.GetRelativePath(parentDir, folder)));
        }
        public static void MoveToCurDirOvr(string folder)
        {
            MoveDirectoryOverwrite(folder, Directory.GetCurrentDirectory());
        }
        public static void MoveOverwrite(string file, string dest)
        {
            MoveBase(file, dest, true);
        }
        public static void MoveBase(string file, string dest, bool overwrite)
        {
            try
            {
                File.Move(file, dest, overwrite);
            }
            catch { }
        }
        public static void MoveDirectoryOverwrite(string dir, string dest)
        {
            try
            {
                Directory.Delete(dest, true);
            }
            catch { }
            MoveDirectoryBase(dir, dest);
        }
        public static void MoveDirectoryBase(string dir, string dest)
        {
            try
            {
                Directory.Move(dir, dest);
            }
            catch { }
        }

        // Yes, the native function exists but it shits itself when one of the files can't be deleted.
        public static void deleteDirectoryRecursive(string path)
        {
            try
            {
                foreach (string file in Directory.GetFiles(path))
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch (IOException) { }
                    catch (UnauthorizedAccessException) { }
                }
                foreach (string dir in Directory.GetDirectories(path))
                {
                    deleteDirectoryRecursive(dir);
                }
            }
            catch { }
        }
    }
}
