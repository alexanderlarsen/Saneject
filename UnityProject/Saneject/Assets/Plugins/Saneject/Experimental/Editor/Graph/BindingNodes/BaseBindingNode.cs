using System;
using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Experimental.Runtime.Bindings;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Experimental.Editor.Graph.BindingNodes
{
    public abstract class BaseBindingNode
    {
        protected BaseBindingNode(BaseBinding binding)
        {
            InterfaceType = binding.InterfaceType;
            ConcreteType = binding.ConcreteType;
            IsCollectionBinding = binding.IsCollectionBinding;
            LocatorStrategySpecified = binding.LocatorStrategySpecified;

            ResolveFromInstances = binding.ResolveFromInstances.ToList();
            Filters = binding.Filters.ToList();
            IdQualifiers = binding.IdQualifiers.ToList();
            MemberNameQualifiers = binding.MemberNameQualifiers.ToList();
            TargetTypeQualifiers = binding.TargetTypeQualifiers.ToList();

            IsValid = binding.IsValid;
        }

        public Type InterfaceType { get; }
        public Type ConcreteType { get; }
        public bool IsCollectionBinding { get; }
        public bool LocatorStrategySpecified { get; }

        public IReadOnlyList<object> ResolveFromInstances { get; }
        public IReadOnlyList<Func<Object, bool>> Filters { get; }
        public IReadOnlyList<string> IdQualifiers { get; }
        public IReadOnlyList<string> MemberNameQualifiers { get; }
        public IReadOnlyList<Type> TargetTypeQualifiers { get; }

        public bool IsValid { get; set; }
    }
}