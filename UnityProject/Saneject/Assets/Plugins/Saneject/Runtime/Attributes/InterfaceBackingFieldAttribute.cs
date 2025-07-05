using System;
using System.ComponentModel;
using UnityEngine;

namespace Plugins.Saneject.Runtime.Attributes
{
    /// <summary>
    /// Warning: Don't use this attribute directly.
    /// Applied by the Roslyn generator <c>SerializeInterfaceGenerator.dll</c> to generated backing fields in partial classes using <see cref="SerializeInterfaceAttribute" /> for serialized interface references.
    /// Stores metadata about the interface type and injection status, enabling the <c>InterfaceBackingFieldPropertyDrawer</c> to render and validate the field.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property), EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class InterfaceBackingFieldAttribute : PropertyAttribute
    {
        public InterfaceBackingFieldAttribute(
            Type interfaceType,
            bool isInjected,
            string injectId)
        {
            InterfaceType = interfaceType;
            IsInjected = isInjected;
            InjectId = injectId;
        }

        public Type InterfaceType { get; }
        public bool IsInjected { get; }
        public string InjectId { get; }
    }
}