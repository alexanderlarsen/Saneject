using System;
using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Experimental.Runtime.Bindings;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Experimental.Editor.Graph.Nodes
{
    public abstract class BindingNode
    {
        // TODO: extract element type from arrays and lists

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
            Filters = binding.Filters.ToList();
            IdQualifiers = binding.IdQualifiers.ToList();
            MemberNameQualifiers = binding.MemberNameQualifiers.ToList();
            TargetTypeQualifiers = binding.TargetTypeQualifiers.ToList();
        }

        public ScopeNode ScopeNode { get; }

        public Type InterfaceType { get; }
        public Type ConcreteType { get; }
        public bool IsCollectionBinding { get; }
        public bool LocatorStrategySpecified { get; }

        public IReadOnlyList<Object> ResolveFromInstances { get; }
        public IReadOnlyList<Func<Object, bool>> Filters { get; }
        public IReadOnlyList<string> IdQualifiers { get; }
        public IReadOnlyList<string> MemberNameQualifiers { get; }
        public IReadOnlyList<Type> TargetTypeQualifiers { get; } 
    }
}