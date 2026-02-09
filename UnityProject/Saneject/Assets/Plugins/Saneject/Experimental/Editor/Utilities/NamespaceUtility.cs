using System.IO;
using System.Linq;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Editor.Utilities
{
    public static class NamespaceUtility
    {
        public static string GetNamespaceFromFullPath(string fullPath)
        {
            string assetsPath = Path.GetFullPath(Application.dataPath);
            string relativePath = fullPath.Replace(assetsPath, "").Trim(Path.DirectorySeparatorChar);

            // Convert to dotted namespace
            string[] parts = relativePath.Split(Path.DirectorySeparatorChar);

            for (int i = 0; i < parts.Length; i++)
            {
                string clean = "";

                // keep only letters, digits, underscore
                foreach (char c in parts[i])
                    if (char.IsLetterOrDigit(c) || c == '_')
                        clean += c;

                // if empty after cleaning, skip
                if (string.IsNullOrEmpty(clean))
                {
                    parts[i] = null;
                    continue;
                }

                // prepend underscore if first char is digit
                if (char.IsDigit(clean[0]))
                    clean = "_" + clean;

                parts[i] = clean;
            }

            string path = string.Join(".", parts.Where(p => !string.IsNullOrEmpty(p)));

            return !string.IsNullOrWhiteSpace(path)
                ? $"namespace {path}"
                : null;
        }
        
        public static string GetNamespaceFromAssetsRelativePath(string relativePath)
        {
            relativePath = relativePath["Assets/".Length..];
            relativePath = relativePath.Replace('\\', '/');

            // Convert to dotted namespace
            string[] parts = relativePath.Split('/');

            for (int i = 0; i < parts.Length; i++)
            {
                string clean = "";

                // keep only letters, digits, underscore
                foreach (char c in parts[i])
                    if (char.IsLetterOrDigit(c) || c == '_')
                        clean += c;

                // if empty after cleaning, skip
                if (string.IsNullOrEmpty(clean))
                {
                    parts[i] = null;
                    continue;
                }

                // prepend underscore if first char is digit
                if (char.IsDigit(clean[0]))
                    clean = "_" + clean;

                parts[i] = clean;
            }

            string path = string.Join(".", parts.Where(p => !string.IsNullOrEmpty(p)));

            return !string.IsNullOrWhiteSpace(path)
                ? $"namespace {path}"
                : null;
        }
    }
}