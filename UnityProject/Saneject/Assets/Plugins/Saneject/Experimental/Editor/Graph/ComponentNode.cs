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
        public ComponentNode(
            Component component,
            TransformNode transformNode)
        {
            Component = component;
            TransformNode = transformNode;

            const BindingFlags bindingFlags =
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Instance |
                BindingFlags.FlattenHierarchy;

            FieldNodes = component
                .GetType()
                .GetFields(bindingFlags)
                .Where(fieldInfo => fieldInfo.HasAttribute<InjectAttribute>())
                .Select(fieldInfo => new FieldNode(fieldInfo, this))
                .ToList();

            MethodNodes = component
                .GetType()
                .GetMethods(bindingFlags)
                .Where(methodInfo => methodInfo.HasAttribute<InjectAttribute>())
                .Select(methodInfo => new MethodNode(methodInfo, this))
                .ToList();
        }

        public Component Component { get; }
        public TransformNode TransformNode { get; }
        public IReadOnlyList<FieldNode> FieldNodes { get; }
        public IReadOnlyList<MethodNode> MethodNodes { get; }
        public bool HasMembers => FieldNodes.Count > 0 || MethodNodes.Count > 0;
    }
}