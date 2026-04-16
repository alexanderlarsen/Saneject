using System;
using System.Collections.Generic;
using Plugins.Saneject.Runtime.Attributes;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using UnityEngine;

namespace Tests.Saneject.Fixtures.Scripts.InjectionTargets
{
    [Serializable]
    public class NestedChildTarget
    {
        [Inject]
        public AssetDependency nestedFieldDependency;

        [field: Inject("nested-property-id", true), SerializeField]
        public AssetDependency NestedPropertyDependency { get; private set; }

        public NestedDeepTarget deepNested = new();
        public AssetDependency NestedMethodAssetDependency { get; private set; }
        public List<ComponentDependency> NestedMethodComponentDependencies { get; private set; }
        public IDependency NestedMethodInterfaceDependency { get; private set; }

        [Inject("nested-method-id")]
        public void InjectNested(
            AssetDependency nestedAssetDependency,
            List<ComponentDependency> nestedComponentDependencies,
            IDependency nestedInterfaceDependency)
        {
            this.NestedMethodAssetDependency = nestedAssetDependency;
            this.NestedMethodComponentDependencies = nestedComponentDependencies;
            this.NestedMethodInterfaceDependency = nestedInterfaceDependency;
        }
    }
}
