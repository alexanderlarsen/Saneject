using Plugins.Saneject.Runtime.Attributes;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using UnityEngine;

namespace Tests.Saneject.Fixtures.Scripts.InjectionTargets
{
    public class ConcreteAssetTarget : MonoBehaviour
    {
        [Inject]
        public AssetDependency dependency;
    }
}