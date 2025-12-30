using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Experimental.Runtime;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Editor.Graph.Nodes
{
    public class TransformNode
    {
        public TransformNode(
            Transform transform,
            TransformNode parentTransformNode = null)
        {
            ParentTransformNode = parentTransformNode;
            Transform = transform;
            ContextNode = new ContextNode(transform);

            ScopeNode = transform.TryGetComponent(out Scope scope)
                ? new ScopeNode(scope, this)
                : null;

            ComponentNodes = transform
                .GetComponents<Component>()
                .Select(component => new ComponentNode(component, this))
                .Where(node => node.HasMembers)
                .ToList();

            ChildTransformNodes = transform
                .Cast<Transform>()
                .Select(child => new TransformNode(child, this))
                .ToList();
        }

        public TransformNode ParentTransformNode { get; }
        public ContextNode ContextNode { get; }
        public ScopeNode ScopeNode { get; }
        public Transform Transform { get; }

        public IReadOnlyList<ComponentNode> ComponentNodes { get; }
        public IReadOnlyList<TransformNode> ChildTransformNodes { get; }
    }
}