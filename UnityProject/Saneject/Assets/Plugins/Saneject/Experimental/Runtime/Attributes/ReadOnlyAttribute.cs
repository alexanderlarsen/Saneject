using System;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Runtime.Attributes
{
    /// <summary>
    /// Helper attribute to gray out a field in the Unity editor.
    /// Used by <c>ReadOnlyPropertyDrawer</c> to disable the field.
    /// Saneject handles this automatically, but you can use this attribute independently.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class ReadOnlyAttribute : PropertyAttribute
    {
    }
}