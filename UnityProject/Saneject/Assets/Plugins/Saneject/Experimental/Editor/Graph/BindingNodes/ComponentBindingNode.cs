using Plugins.Saneject.Experimental.Runtime.Bindings.Component;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Editor.Graph.BindingNodes
{
    public class ComponentBindingNode : BaseBindingNode
    {
        public ComponentBindingNode(ComponentBinding binding) : base(binding)
        {
            SearchOrigin = binding.SearchOrigin;
            SearchDirection = binding.SearchDirection;
            FindObjectsInactive = binding.FindObjectsInactive;
            FindObjectsSortMode = binding.FindObjectsSortMode;

            CustomTargetTransform = binding.CustomTargetTransform;
            IncludeSelfInSearch = binding.IncludeSelfInSearch;
            ChildIndexForSearch = binding.ChildIndexForSearch;
            ResolveFromProxy = binding.ResolveFromProxy;
        }

        public SearchOrigin SearchOrigin { get; }
        public SearchDirection SearchDirection { get; }
        public FindObjectsInactive FindObjectsInactive { get; }
        public FindObjectsSortMode FindObjectsSortMode { get; }

        public Transform CustomTargetTransform { get; }
        public bool IncludeSelfInSearch { get; }
        public int ChildIndexForSearch { get; }
        public bool ResolveFromProxy { get; }
    }
}