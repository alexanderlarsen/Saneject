using System;
using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Experimental.Editor.Extensions;
using Plugins.Saneject.Experimental.Runtime.Bindings;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Experimental.Editor.Graph.Nodes
{
    public abstract class BindingNode : IEquatable<BindingNode>
    {
        protected BindingNode(
            Binding binding,
            ScopeNode scopeNode)
        {
            ScopeNode = scopeNode;
            InterfaceType = binding.InterfaceType;
            ConcreteType = binding.ConcreteType;
            IsCollectionBinding = binding.IsCollectionBinding;
            LocatorStrategySpecified = binding.LocatorStrategySpecified;
            ResolveFromInstances = binding.ResolveFromInstances.ToList();
            DependencyFilters = binding.DependencyFilters.ToList();
            IdQualifiers = binding.IdQualifiers.ToList();
            MemberNameQualifiers = binding.MemberNameQualifiers.ToList();
            TargetTypeQualifiers = binding.TargetTypeQualifiers.ToList();
        }

        public ScopeNode ScopeNode { get; }
        public Type InterfaceType { get; }
        public Type ConcreteType { get; }
        public bool IsCollectionBinding { get; }
        public bool LocatorStrategySpecified { get; }

        public IReadOnlyList<DependencyFilter> DependencyFilters { get; }
        public IReadOnlyList<Object> ResolveFromInstances { get; }
        public IReadOnlyList<string> IdQualifiers { get; }
        public IReadOnlyList<string> MemberNameQualifiers { get; }
        public IReadOnlyList<Type> TargetTypeQualifiers { get; }

        public bool Equals(BindingNode other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (!Equals(ScopeNode, other.ScopeNode))
                return false;

            if (other.GetType() != GetType())
                return false;

            if (InterfaceType != null ? InterfaceType != other.InterfaceType : ConcreteType != other.ConcreteType)
                return false;

            if (IsCollectionBinding != other.IsCollectionBinding)
                return false;

            if (TargetTypeQualifiers.Count == 0
                && MemberNameQualifiers.Count == 0
                && IdQualifiers.Count == 0
                && other.TargetTypeQualifiers.Count == 0
                && other.MemberNameQualifiers.Count == 0
                && other.IdQualifiers.Count == 0)
                return true;

            return TargetTypeQualifiers.OverlapsWith(other.TargetTypeQualifiers)
                   && MemberNameQualifiers.OverlapsWith(other.MemberNameQualifiers)
                   && IdQualifiers.OverlapsWith(other.IdQualifiers);
        }

        public override bool Equals(object obj)
        {
            return obj is BindingNode bindingNode && Equals(bindingNode);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine
            (
                ScopeNode,
                GetType(),
                InterfaceType ?? ConcreteType,
                IsCollectionBinding,
                TargetTypeQualifiers.Count > 0,
                MemberNameQualifiers.Count > 0,
                IdQualifiers.Count > 0
            );
        }
    }
}