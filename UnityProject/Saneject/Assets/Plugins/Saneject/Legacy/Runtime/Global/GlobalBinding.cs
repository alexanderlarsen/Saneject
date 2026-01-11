using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Legacy.Runtime.Global
{
    /// <summary>
    /// Serializable data for one global binding: stores the type and the <see cref="object" /> instance.
    /// </summary>
    [Serializable]
    public class GlobalBinding
    {
        [SerializeField]
        private Object instance;

        private Type cachedType;

        public GlobalBinding(Object obj)
        {
            instance = obj;
        }

        public Type Type => cachedType ??= instance.GetType();
        public Object Instance => instance;
    }
}