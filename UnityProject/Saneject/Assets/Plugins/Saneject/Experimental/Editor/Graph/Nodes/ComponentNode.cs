using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Plugins.Saneject.Editor.Extensions;
using Plugins.Saneject.Experimental.Editor.Extensions;
using Plugins.Saneject.Runtime.Attributes;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Editor.Graph.Nodes
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
                .GetFieldsDeep(bindingFlags)
                .Where(result =>
                    result.FieldInfo.HasAttribute<InjectAttribute>()
                    // TODO: remove when getting rid of InterfaceBackingFieldAttribute
                    && !result.FieldInfo.HasAttribute<InterfaceBackingFieldAttribute>())
                .Select(result => new FieldNode
                (
                    owner: result.Owner,
                    fieldInfo: result.FieldInfo,
                    componentNode: this,
                    pathFromComponent: result.Path
                ))
                .ToList();

            MethodNodes = component
                .GetMethodsDeep(bindingFlags)
                .Where(result => result.MethodInfo.HasAttribute<InjectAttribute>())
                .Select(result => new MethodNode
                (
                    owner: result.Owner,
                    methodInfo: result.MethodInfo,
                    componentNode: this,
                    pathFromComponent: result.Path
                ))
                .ToList();
        }

        public Component Component { get; }
        public TransformNode TransformNode { get; }
        public IReadOnlyList<FieldNode> FieldNodes { get; }
        public IReadOnlyList<MethodNode> MethodNodes { get; }
        public bool HasMembers => FieldNodes.Count > 0 || MethodNodes.Count > 0;
    }
}