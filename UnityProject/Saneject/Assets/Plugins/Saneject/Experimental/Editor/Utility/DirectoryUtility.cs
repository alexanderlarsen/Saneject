using System;
using System.IO;

namespace Plugins.Saneject.Experimental.Editor.Utility
{
    public static class DirectoryUtility
    {
        public static void EnsureDirectoryExists(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("Path cannot be null or empty.", nameof(path));

            string directoryName = Path.GetDirectoryName(path);

            if (string.IsNullOrEmpty(directoryName))
                throw new ArgumentException("Path is not a valid file path.", nameof(path));

            if (!Directory.Exists(directoryName))
                Directory.CreateDirectory(directoryName);
        }
    }
} 