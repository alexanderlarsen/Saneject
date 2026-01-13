using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Editor.Inspectors.Models
{
    public sealed class ComponentModel
    {
        public ComponentModel(
            Object target,
            SerializedObject serializedObject)
        {
            SerializedObject = serializedObject;
            Target = target;

            Properties = target
                .GetType()
                .GetFieldInfosForInspector()
                .Select(field => new PropertyModel(serializedObject, field))
                .Where(p => p.SerializedProperty != null)
                .ToList();
        }

        public Object Target { get; }
        public SerializedObject SerializedObject { get; }
        public IReadOnlyList<PropertyModel> Properties { get; }
    }
}