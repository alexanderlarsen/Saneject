using UnityEngine;

namespace Plugins.Saneject.Runtime.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="Component"/>.
    /// </summary>
    public static class ComponentExtensions
    {
        /// <summary>
        /// Destroy this <see cref="Component"/> if other sibling components exist; destroy its <see cref="GameObject"/> if this is the only component (besides <see cref="Transform"/>).
        /// </summary>
        public static void DestroySelfOrGameObjectIfSolo(this Component component)
        {
            if (component.GetComponents<Component>().Length <= 2)
                Object.DestroyImmediate(component.gameObject);
            else
                Object.DestroyImmediate(component);
        }

        public static int GetComponentIndexCompat(this Component component)
        {
#if UNITY_2023_1_OR_NEWER
            return component.GetComponentIndex();
#else
            var all = component.gameObject.GetComponents<Component>();
            return System.Array.IndexOf(all, component);
#endif
        }
    }
}