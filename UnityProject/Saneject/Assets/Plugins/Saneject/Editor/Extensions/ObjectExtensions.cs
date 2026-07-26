using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace Plugins.Saneject.Editor.Extensions
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ObjectExtensions
    {
        public static IEnumerable<T> AsEnumerable<T>(this T obj)
        {
            yield return obj;
        }
        
        public static T[] AsArray<T>(this T obj)
        {
            return new[] { obj };
        }
        
        public static long GetInstanceIDCompat(this Object obj)
        {
#if UNITY_6000_4_OR_NEWER
            return (long)EntityId.ToULong(obj.GetEntityId());
#else
            return (int)obj.GetInstanceID();
#endif
        }

        // Returns the object id boxed as the exact type expected by reflection calls
        // against APIs whose signature changed from int to EntityId in Unity 6000.4
        // (e.g. SceneHierarchyWindow.SetExpanded). Do not use for identity keys.
        public static object GetInstanceIDBoxedCompat(this Object obj)
        {
#if UNITY_6000_4_OR_NEWER
            return obj.GetEntityId();
#else
            return obj.GetInstanceID();
#endif
        }
    }
}