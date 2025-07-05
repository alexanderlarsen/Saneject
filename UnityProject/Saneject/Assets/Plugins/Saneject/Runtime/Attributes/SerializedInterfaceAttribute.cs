using System;

namespace Plugins.Saneject.Runtime.Attributes
{
    /// <summary>
    /// Marks an interface field for serialization support.
    /// Tells the <c>SerializeInterfaceGenerator.dll</c> Roslyn generator to generate a partial class for the owner with a backing <see cref="UnityEngine.Object" /> field,
    /// assigning it to the interface during deserialization and enabling Unity to (indirectly) show serialized interface references in the inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class SerializeInterfaceAttribute : Attribute
    {
    }
}