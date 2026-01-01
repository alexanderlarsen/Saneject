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

        public List<Object> ResolveFromInstances { get; set; } = new();
        public List<Func<Object, bool>> Filters { get; set; } = new();
        public List<string> IdQualifiers { get; set; } = new();
        public List<string> MemberNameQualifiers { get; set; } = new();
        public List<Type> TargetTypeQualifiers { get; set; } = new();
    }
}