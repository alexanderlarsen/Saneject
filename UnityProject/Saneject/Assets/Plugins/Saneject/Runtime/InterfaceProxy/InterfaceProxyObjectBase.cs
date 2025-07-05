using UnityEngine;

namespace Plugins.Saneject.Runtime.InterfaceProxy
{
    /// <summary>
    /// Non-generic base class for <see cref="InterfaceProxyObject{TConcrete}"/>. 
    /// Required so that Unity Editor scripts can target proxy assets regardless of generic type.
    /// </summary>
    public abstract class InterfaceProxyObjectBase : ScriptableObject
    {
    }
}