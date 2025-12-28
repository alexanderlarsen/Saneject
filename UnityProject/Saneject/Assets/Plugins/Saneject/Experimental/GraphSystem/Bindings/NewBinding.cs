using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Experimental.GraphSystem.Bindings
{
    public class NewBinding
    {
        private readonly List<Func<Object, bool>> filters = new();
        private readonly List<string> idQualifiers = new();
        private readonly List<string> injectionTargetMemberNameQualifiers = new();
        private readonly List<Type> injectionTargetTypeQualifiers = new();

        public Type InterfaceType { get; private set; }
        public Type ConcreteType { get; private set; }
        public BindingType BindingType { get; private set; }

        public SearchOrigin SearchOrigin { get; private set; }
        public SearchDirection SearchDirection { get; private set; }

        public Transform CustomTargetTransform { get; private set; }
        public List<object> DirectInstancesToResolveFrom { get; private set; } = new();

        public bool IncludeSelfInSearch { get; private set; }
        public int ChildIndexForSearch { get; private set; }
        public bool ResolveFromProxy { get; private set; }

        public IReadOnlyList<string> IdQualifiers => idQualifiers;
        public IReadOnlyList<Type> InjectionTargetTypeQualifiers => injectionTargetTypeQualifiers;
        public IReadOnlyList<string> InjectionTargetMemberNameQualifiers => injectionTargetMemberNameQualifiers;

        public FindObjectsSettings SceneSearchFindObjectsSettings { get; private set; } 

        public void SetTargetType(Type targetType)
        {
            if (targetType.IsInterface)
                InterfaceType = targetType;
            else
                ConcreteType = targetType;
        }

        public void MarkResolveFromProxy()
        {
            ResolveFromProxy = true;
        }

        public void SetCustomTargetTransform(Transform customTargetTransform)
        {
            CustomTargetTransform = customTargetTransform;
        }

        public void SetBindingType(BindingType bindingType)
        {
            BindingType = bindingType;
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

        public void ResolveFromInstances(params object[] instances)
        {
            DirectInstancesToResolveFrom.AddRange(instances);
        }

        public void AddIdQualifier(string id)
        {
            idQualifiers.Add(id);
        }

        public void AddInjectionTargetTypeQualifier(Type type)
        {
            injectionTargetTypeQualifiers.Add(type);
        }

        public void AddInjectionTargetMemberNameQualifier(string memberName)
        {
            injectionTargetMemberNameQualifiers.Add(memberName);
        }

        public void SetSceneSearchFindObjectsSettings(
            FindObjectsInactive includeInactive,
            FindObjectsSortMode sortMode)
        {
            SceneSearchFindObjectsSettings = new FindObjectsSettings(includeInactive, sortMode);
        }

        /// <summary>
        /// Add a filter for candidate <see cref="UnityEngine.Object" />s. Only objects passing all filters will be considered for resolution.
        /// </summary>
        /// <param name="filter">Predicate to evaluate each <see cref="UnityEngine.Object" />.</param>
        public void AddFilter(Func<Object, bool> filter)
        {
            filters.Add(filter);
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