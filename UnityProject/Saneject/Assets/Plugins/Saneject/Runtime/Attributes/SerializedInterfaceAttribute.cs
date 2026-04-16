using System;

namespace Plugins.Saneject.Runtime.Attributes
{
    /// <summary>
    /// Marks an interface-typed member for Saneject's serialized-interface code generation.
    /// </summary>
    /// <remarks>
    /// Apply this attribute to interface fields, interface arrays, interface lists, or auto-properties via <c>[field: SerializeInterface]</c>.
    /// Saneject generates hidden <see cref="UnityEngine.Object"/> backing members plus serialization sync code so Unity can display and persist
    /// interface references in scenes and prefabs.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class SerializeInterfaceAttribute : Attribute
    {
    }
}
