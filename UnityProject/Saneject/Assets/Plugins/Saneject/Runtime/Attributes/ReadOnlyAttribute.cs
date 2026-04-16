using System;
using UnityEngine;

namespace Plugins.Saneject.Runtime.Attributes
{
    /// <summary>
    /// Marks a field as read-only in the Unity inspector.
    /// </summary>
    /// <remarks>
    /// When applied to a serialized field, this attribute causes the field to appear grayed out and disabled in the inspector.
    /// Saneject automatically draws <see cref="InjectAttribute" /> fields as read-only, but you can use this attribute independently on non-injected fields.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field)]
    public class ReadOnlyAttribute : PropertyAttribute
    {
    }
}