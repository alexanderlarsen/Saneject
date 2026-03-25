using System.Collections.Generic;
using Plugins.Saneject.Runtime.Attributes;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using UnityEngine;

namespace Tests.Saneject.Fixtures.Scripts.InjectionTargets
{
    public class GraphMetadataTarget : MonoBehaviour
    {
        [Inject("field-id", true)]
        public ComponentDependency fieldDependency;

        [field: Inject("property-id", true), SerializeField]
        public List<ComponentDependency> PropertyDependencies { get; private set; }

        public NestedChildTarget nested = new();
        public NestedChildTarget nullNested;

        [Inject("method-id", true)]
        private void InjectTopLevel(
            ComponentDependency singleDependency,
            IDependency interfaceDependency,
            ComponentDependency[] arrayDependencies,
            List<IDependency> listDependencies)
        {
        }
    }
}
