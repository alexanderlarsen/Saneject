using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Plugins.Saneject.Editor.Extensions;
using Plugins.Saneject.Runtime.Attributes;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Editor.Graph
{
    public class ComponentNode
    {
        public ComponentNode(Component component)
        {
            Component = component;

            const BindingFlags bindingFlags =
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Instance |
                BindingFlags.FlattenHierarchy;

            Fields = component
                .GetType()
                .GetFields(bindingFlags)
                .Where(fieldInfo => fieldInfo.HasAttribute<InjectAttribute>())
                .Select(fieldInfo => new FieldNode(component, fieldInfo))
                .ToList();

            Methods = component
                .GetType()
                .GetMethods(bindingFlags)
                .Where(methodInfo => methodInfo.HasAttribute<InjectAttribute>())
                .Select(methodInfo => new MethodNode(component, methodInfo))
                .ToList();
        }

        public Component Component { get; }
        public IReadOnlyList<FieldNode> Fields { get; }
        public IReadOnlyList<MethodNode> Methods { get; }
        public bool HasMembers => Fields.Count > 0 || Methods.Count > 0;
    }
}