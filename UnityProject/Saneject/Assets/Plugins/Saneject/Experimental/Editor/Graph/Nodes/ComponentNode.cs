using System.Collections.Generic;
using System.Linq;
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

            FieldNodes = component
                .EnumerateInjectionFieldsDeep()
                .Where(result => !result.FieldInfo.HasAttribute<InterfaceBackingFieldAttribute>()) // TODO: remove when getting rid of InterfaceBackingFieldAttribute
                .Select(result => new FieldNode
                (
                    owner: result.Owner,
                    fieldInfo: result.FieldInfo,
                    componentNode: this,
                    pathFromComponent: result.Path,
                    injectAttribute: result.InjectAttribute
                ))
                .ToList();

            MethodNodes = component
                .EnumerateInjectionMethodsDeep()
                .Select(result => new MethodNode
                (
                    owner: result.Owner,
                    methodInfo: result.MethodInfo,
                    componentNode: this,
                    pathFromComponent: result.Path,
                    injectAttribute: result.InjectAttribute
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