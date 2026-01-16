using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Experimental.Editor.Data;
using Plugins.Saneject.Experimental.Editor.Extensions;
using Plugins.Saneject.Experimental.Runtime;
using Plugins.Saneject.Experimental.Runtime.Scopes;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Editor.Graph.Nodes
{
    public class TransformNode
    {
        public TransformNode(
            Transform transform,
            Transform[] startTransforms,
            TransformNode parentTransformNode = null)
        {
            ParentTransformNode = parentTransformNode;
            Transform = transform;
            ContextIdentity = new ContextIdentity(transform);

            DeclaredScopeNode = transform.TryGetComponent(out Scope scope)
                ? new ScopeNode(scope, this)
                : null;

            NearestScopeNode = this.FindNearestScope();

            ComponentNodes = transform
                .GetComponents<Component>()
                .Select(component => new ComponentNode(component, this))
                .Where(node => node.HasMembers)
                .ToList();

            ChildTransformNodes = transform
                .Cast<Transform>()
                .Select(child => new TransformNode(child, startTransforms, this))
                .ToList();
        }

        public Transform Transform { get; }
        public TransformNode ParentTransformNode { get; }
        public ContextIdentity ContextIdentity { get; }
        public ScopeNode NearestScopeNode { get; }
        public ScopeNode DeclaredScopeNode { get; }
        public IReadOnlyCollection<ComponentNode> ComponentNodes { get; }
        public IReadOnlyCollection<TransformNode> ChildTransformNodes { get; }
    }
}