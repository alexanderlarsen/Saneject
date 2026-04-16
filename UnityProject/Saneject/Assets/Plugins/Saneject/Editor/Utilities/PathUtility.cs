using System.ComponentModel;
using System.Linq;
using System.Text;
using UnityEngine;
using Component = UnityEngine.Component;

namespace Plugins.Saneject.Editor.Utilities
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class PathUtility
    {
        private static readonly char[] IllegalPathChars =
        {
            '<',
            '>',
            ':',
            '"',
            '|',
            '?',
            '*'
        };

        public static string SanitizeFolderPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return path;

            path = path.Replace('\\', '/');
            StringBuilder sb = new(path.Length);

            foreach (char c in path.Where(c => !IllegalPathChars.Contains(c)))
                sb.Append(c);

            return sb.ToString();
        }

        public static string GetComponentPath(Component component)
        {
            string goPath = GetTransformPath(component.transform);
            return $"{goPath}/{component.GetType().Name}";
        }

        /// <summary>
        /// Returns the full GameObject hierarchy path from the scene root to this transform.
        /// Example: "Root/Child/Leaf".
        /// </summary>
        private static string GetTransformPath(Transform transform)
        {
            return !transform.parent ? transform.name : $"{GetTransformPath(transform.parent)}/{transform.name}";
        }
    }
}