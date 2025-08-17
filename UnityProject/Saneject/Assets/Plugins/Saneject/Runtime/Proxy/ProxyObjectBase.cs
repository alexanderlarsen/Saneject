using UnityEngine;

namespace Plugins.Saneject.Runtime.Proxy
{
    /// <summary>
    /// Non-generic base class for <see cref="Plugins.Saneject.Runtime.Proxy.ProxyObject{TConcrete}" />.
    /// Required so that Unity Editor scripts can target proxy assets regardless of generic type.
    /// </summary>
    public abstract class ProxyObjectBase : ScriptableObject
    {
    }
}