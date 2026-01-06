using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Experimental.Runtime.Bindings
{
    public abstract class Binding
    {
        public Type InterfaceType { get; set; }
        public Type ConcreteType { get; set; }
        public bool IsCollectionBinding { get; set; }
        public bool LocatorStrategySpecified { get; set; }

        public List<Object> ResolveFromInstances { get; } = new();
        public List<string> IdQualifiers { get; } = new();
        public List<string> MemberNameQualifiers { get; } = new();
        public List<Type> TargetTypeQualifiers { get; } = new();
        public List<DependencyFilter> DependencyFilters { get; } = new();
    }
}