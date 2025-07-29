using UnityEngine;

namespace Plugins.Saneject.Runtime.Bindings
{
    /// <summary>
    /// Abstract base class providing shared helper methods for filter builders in Saneject.
    /// Used by filter builders to extract common Unity object data such as <see cref="Transform" />, <see cref="GameObject" />, and names from <see cref="UnityEngine.Object" /> instances.
    /// </summary>
    public abstract class BaseFilterBuilder
    {
        #region STATIC HELPER METHODS

        protected static Transform GetTransform(Object o)
        {
            return o switch
            {
                Transform t => t,
                Component c => c.transform,
                GameObject go => go.transform,
                _ => null
            };
        }

        protected static string GetObjectName(Object o)
        {
            return o switch
            {
                Component c => c.gameObject.name,
                GameObject go => go.name,
                _ => o?.name
            };
        }

        protected static GameObject GetGameObject(Object o)
        {
            return o switch
            {
                GameObject go => go,
                Component c => c.gameObject,
                _ => null
            };
        }

        #endregion
    }
}