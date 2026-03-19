using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Plugins.Saneject.Editor.Data.Context;
using Plugins.Saneject.Runtime.Scopes;
using UnityEngine;

// ReSharper disable LoopCanBePartlyConvertedToQuery

namespace Plugins.Saneject.Editor.Inspectors.Models
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class ScopeHierarchyModel
    {
        public ScopeHierarchyModel(
            Scope scope,
            Scope inspectedScope,
            ContextIdentity inspectedScopeContextIdentity)
        {
            ScopeName = scope.GetType().Name;
            GameObject = scope.gameObject;
            IsCurrent = scope == inspectedScope;
            ContextIdentity = new ContextIdentity(scope);
            IsSameContext = ContextIdentity.Equals(inspectedScopeContextIdentity);

            Children = scope.transform
                .Cast<Transform>()
                .SelectMany(t => EnumerateScopes(t, inspectedScope, inspectedScopeContextIdentity))
                .ToList();
        }

        public string ScopeName { get; }
        public GameObject GameObject { get; }
        public bool IsCurrent { get; }
        public ContextIdentity ContextIdentity { get; }
        public bool IsSameContext { get; }
        public IReadOnlyList<ScopeHierarchyModel> Children { get; }

        private static IEnumerable<ScopeHierarchyModel> EnumerateScopes(
            Transform transform,
            Scope inspectedScope,
            ContextIdentity inspectedScopeContextIdentity)
        {
            if (transform.TryGetComponent(out Scope scope))
            {
                yield return new ScopeHierarchyModel(scope, inspectedScope, inspectedScopeContextIdentity);
                yield break;
            }

            foreach (Transform child in transform)
                foreach (ScopeHierarchyModel model in EnumerateScopes(child, inspectedScope, inspectedScopeContextIdentity))
                    yield return model;
        }
    }
}