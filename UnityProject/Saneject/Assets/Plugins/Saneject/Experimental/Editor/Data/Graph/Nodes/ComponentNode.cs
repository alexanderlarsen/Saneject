using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Experimental.Editor.Extensions;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Editor.Data.Graph.Nodes
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
                .GetInjectionFieldsDeep()
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
                .GetInjectionMethodsDeep()
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
        public IReadOnlyCollection<FieldNode> FieldNodes { get; }
        public IReadOnlyCollection<MethodNode> MethodNodes { get; }
        public bool HasMembers => FieldNodes.Count > 0 || MethodNodes.Count > 0;
    }
}