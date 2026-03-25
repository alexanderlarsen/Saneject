using System;
using System.Collections.Generic;
using Plugins.Saneject.Runtime.Attributes;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using UnityEngine;

namespace Tests.Saneject.Fixtures.Scripts.InjectionTargets
{
    [Serializable]
    public class GraphMetadataNested
    {
        [Inject]
        public AssetDependency nestedFieldDependency;

        [field: Inject("nested-property-id", true), SerializeField]
        public AssetDependency NestedPropertyDependency { get; private set; }

        public GraphMetadataNestedChild deepNested = new();

        [Inject("nested-method-id")]
        private void InjectNested(
            AssetDependency nestedAssetDependency,
            List<ComponentDependency> nestedComponentDependencies,
            IDependency nestedInterfaceDependency)
        {
        }
    }
}
