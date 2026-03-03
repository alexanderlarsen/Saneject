using System;
using System.Collections.Generic;
using System.ComponentModel;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Experimental.Runtime.Bindings
{
    /// <summary>
    /// Base class for all bindings.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class Binding
    {
        /// <summary>
        /// The interface or abstract type that this binding satisfies.
        /// </summary>
        public Type InterfaceType { get; set; }

        /// <summary>
        /// The concrete type to resolve.
        /// </summary>
        public Type ConcreteType { get; set; }

        /// <summary>
        /// Indicates whether this binding represents a list or array.
        /// </summary>
        public bool IsCollectionBinding { get; set; }

        /// <summary>
        /// Indicates whether a specific locator strategy has been defined for this binding.
        /// </summary>
        public bool LocatorStrategySpecified { get; set; }

        /// <summary>
        /// User supplied instances to use for resolution.
        /// </summary>
        public List<Object> ResolveFromInstances { get; } = new();

        /// <summary>
        /// A list of ID qualifiers used to restrict the binding to members that specify the same IDs in their Inject-attributes.
        /// </summary>
        public List<string> IdQualifiers { get; } = new();

        /// <summary>
        /// A list of member name qualifiers used to restrict the binding to specific members.
        /// </summary>
        public List<string> MemberNameQualifiers { get; } = new();

        /// <summary>
        /// A list of target type qualifiers used to restrict the binding to specific consuming types.
        /// </summary>
        public List<Type> TargetTypeQualifiers { get; } = new();

        /// <summary>
        /// A list of <see cref="DependencyFilter"/> used to further refine the selection of dependencies.
        /// </summary>
        public List<DependencyFilter> DependencyFilters { get; } = new();
    }
}