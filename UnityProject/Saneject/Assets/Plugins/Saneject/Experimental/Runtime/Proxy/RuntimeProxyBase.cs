using UnityEngine;

namespace Plugins.Saneject.Experimental.Runtime.Proxy
{
    /// <summary>
    /// Non-generic base class for <see cref="RuntimeProxy{TConcrete}" />.
    /// Required so that Unity Editor scripts can target proxy assets regardless of generic type.
    /// </summary>
    public abstract class RuntimeProxyBase : ScriptableObject
    {
    }
}