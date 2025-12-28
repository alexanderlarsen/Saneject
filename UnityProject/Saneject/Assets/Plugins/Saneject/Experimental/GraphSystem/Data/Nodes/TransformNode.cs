using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Plugins.Saneject.Experimental.GraphSystem.Data.Nodes
{
    public class TransformNode
    {
        public TransformNode(
            Transform transform,
            TransformNode parent = null)
        {
            Parent = parent;
            Transform = transform;
            Context = new ContextNode(transform);

            Scope = transform.TryGetComponent(out NewScope scope)
                ? new ScopeNode(scope, this)
                : null;

            Components = transform
                .GetComponents<Component>()
                .Select(component => new ComponentNode(component))
                .Where(node => node.HasMembers)
                .ToList();

            Children = transform
                .Cast<Transform>()
                .Select(child => new TransformNode(child, this))
                .ToList();
        }

        public TransformNode Parent { get; }
        public Transform Transform { get; }
        public ContextNode Context { get; }
        public ScopeNode Scope { get; }
        public IReadOnlyList<ComponentNode> Components { get; }
        public IReadOnlyList<TransformNode> Children { get; }
    }
}