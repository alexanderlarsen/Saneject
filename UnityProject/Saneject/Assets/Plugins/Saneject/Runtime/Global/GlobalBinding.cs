using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Runtime.Global
{
    /// <summary>
    /// Serializable data for one global binding: stores the type and the <see cref="object" /> instance.
    /// </summary>
    [Serializable]
    public class GlobalBinding
    {
        [SerializeField]
        private Object instance;

        public GlobalBinding(Object obj)
        {
            instance = obj;
        }

        public Type Type => instance != null ? instance.GetType() : null;
        public Object Instance => instance;
    }
}