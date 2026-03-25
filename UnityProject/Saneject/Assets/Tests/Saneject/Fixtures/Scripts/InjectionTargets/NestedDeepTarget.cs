using System;
using Plugins.Saneject.Runtime.Attributes;
using Tests.Saneject.Fixtures.Scripts.Dependencies;

namespace Tests.Saneject.Fixtures.Scripts.InjectionTargets
{
    [Serializable]
    public class GraphMetadataNestedChild
    {
        [Inject("deep-field-id", true)]
        public AssetDependency deepFieldDependency;
    }
}
