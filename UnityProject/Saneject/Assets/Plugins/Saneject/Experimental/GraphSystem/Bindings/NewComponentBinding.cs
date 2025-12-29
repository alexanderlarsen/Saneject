using UnityEngine;

namespace Plugins.Saneject.Experimental.GraphSystem.Bindings
{
    public class NewComponentBinding : NewBinding
    {
        public SearchOrigin SearchOrigin { get; private set; }
        public SearchDirection SearchDirection { get; private set; }

        public Transform CustomTargetTransform { get; private set; }
        public bool IncludeSelfInSearch { get; private set; }
        public int ChildIndexForSearch { get; private set; }
        public FindObjectsSettings SceneSearchFindObjectsSettings { get; private set; }
        public bool ResolveFromProxy { get; private set; }
   
        
        public void MarkResolveFromProxy()
        {
            ResolveFromProxy = true;
        }

        public void SetCustomTargetTransform(Transform customTargetTransform)
        {
            CustomTargetTransform = customTargetTransform;
        }

        public void SetSearchParameters(
            SearchOrigin origin,
            SearchDirection direction)
        {
            SearchOrigin = origin;
            SearchDirection = direction;
        }

        public void SetChildIndexForSearch(int childIndex)
        {
            ChildIndexForSearch = childIndex;
        }

        public void SetIncludeSelfInSearch(bool includeSelf)
        {
            IncludeSelfInSearch = includeSelf;
        }

        public void SetSceneSearchFindObjectsSettings(
            FindObjectsInactive includeInactive,
            FindObjectsSortMode sortMode)
        {
            SceneSearchFindObjectsSettings = new FindObjectsSettings(includeInactive, sortMode);
        }
 
        public class FindObjectsSettings
        {
            public FindObjectsSettings(
                FindObjectsInactive includeInactive,
                FindObjectsSortMode sortMode)
            {
                IncludeInactive = includeInactive;
                SortMode = sortMode;
            }

            public FindObjectsInactive IncludeInactive { get; }
            public FindObjectsSortMode SortMode { get; }
        }
    }
}