using System;

namespace Plugins.Saneject.Runtime.Attributes
{
    /// <summary>
    /// Creates a Roslyn-generated backing field for an interface-typed field to make it show up in the inspector and behave like a normal serialized field.
    /// </summary>
    /// <remarks>
    /// Applying this attribute to an interface field causes the Roslyn generator to create a backing <see cref="UnityEngine.Object"/> field
    /// in a partial class and sync it with the real interface field during serialization and deserialization, allowing Unity to display
    /// interface references in the inspector and persist them with the scene or prefab.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class SerializeInterfaceAttribute : Attribute
    {
    }
}