using System.ComponentModel;
using Plugins.Saneject.Experimental.Runtime.Proxy;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Runtime.Bindings.Component
{
    /// <summary>
    /// Represents a binding to a Unity <see cref="Component"/>.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class ComponentBinding : Binding
    {
        /// <summary>
        /// Specifies the origin type for the component search.
        /// </summary>
        public SearchOrigin SearchOrigin { get; set; }

        /// <summary>
        /// Specifies the direction to search for the component from the <see cref="SearchOrigin"/>.
        /// </summary>
        public SearchDirection SearchDirection { get; set; }

        /// <summary>
        /// Specifies whether to include inactive objects in the search.
        /// </summary>
        public FindObjectsInactive FindObjectsInactive { get; set; }

        /// <summary>
        /// Specifies the sort mode for the search results.
        /// </summary>
        public FindObjectsSortMode FindObjectsSortMode { get; set; }

        /// <summary>
        /// A custom <see cref="Transform"/> used as the search origin when <see cref="SearchOrigin"/> is set to <see cref="SearchOrigin.CustomTargetTransform"/>.
        /// </summary>
        public Transform CustomTargetTransform { get; set; }

        /// <summary>
        /// Indicates whether to include the origin <see cref="Transform"/> itself in the search.
        /// </summary>
        public bool IncludeSelfInSearch { get; set; }

        /// <summary>
        /// The index of the child to search for when <see cref="SearchDirection"/> is set to <see cref="SearchDirection.ChildAtIndex"/>.
        /// </summary>
        public int ChildIndexForSearch { get; set; }

        /// <summary>
        /// The <see cref="RuntimeProxyConfig"/> associated with this binding - if any.
        /// </summary>
        public RuntimeProxyConfig RuntimeProxyConfig { get; set; }
    }
}