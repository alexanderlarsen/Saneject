using System.Collections.Generic;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Runtime.Bindings.Component
{
    public class ComponentBinding : Binding
    {
        public SearchOrigin SearchOrigin { get; set; }
        public SearchDirection SearchDirection { get; set; }
        public FindObjectsInactive FindObjectsInactive { get; set; }
        public FindObjectsSortMode FindObjectsSortMode { get; set; }

        public Transform CustomTargetTransform { get; set; }
        public bool IncludeSelfInSearch { get; set; }
        public int ChildIndexForSearch { get; set; }
        public bool ResolveFromProxy { get; set; }
    }
}