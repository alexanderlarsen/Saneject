using UnityEngine;

namespace Plugins.SanejectLegacy.Runtime.Proxy
{
    /// <summary>
    /// Non-generic base class for <see cref="ProxyObject{TConcrete}" />.
    /// Required so that Unity Editor scripts can target proxy assets regardless of generic type.
    /// </summary>
    public abstract class ProxyObjectBase : ScriptableObject
    {
    }
}