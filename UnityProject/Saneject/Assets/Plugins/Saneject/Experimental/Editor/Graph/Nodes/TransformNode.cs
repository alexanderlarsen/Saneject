using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Experimental.Runtime;
using Plugins.Saneject.Runtime.Settings;
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

            DeclaredScopeNode = transform.TryGetComponent(out Scope scope)
                ? new ScopeNode(scope, this)
                : null;

            NearestScopeNode = FindNearestSameContextScope();

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
        public ScopeNode DeclaredScopeNode { get; }
        public ScopeNode NearestScopeNode { get; }
        public Transform Transform { get; }

        public IReadOnlyList<ComponentNode> ComponentNodes { get; }
        public IReadOnlyList<TransformNode> ChildTransformNodes { get; }

        private ScopeNode FindNearestSameContextScope()
        {
            TransformNode current = this;

            while (current != null)
            {
                ScopeNode scope = current.DeclaredScopeNode;

                if (scope != null &&
                    (!UserSettings.UseContextIsolation ||
                     current.ContextNode == ContextNode))
                    return scope;

                current = current.ParentTransformNode;
            }

            return null;
        }
    }
}