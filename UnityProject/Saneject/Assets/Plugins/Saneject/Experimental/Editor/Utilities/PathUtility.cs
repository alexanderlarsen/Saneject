using UnityEngine;

namespace Plugins.Saneject.Experimental.Editor.Utilities
{
    public static class PathUtility
    {
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