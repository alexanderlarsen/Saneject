using Plugins.Saneject.Runtime.Attributes;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using UnityEngine;

namespace Tests.Saneject.Fixtures.Scripts.InjectionTargets
{
    public class SingleConcreteAssetTargetWithDifferentMemberName : MonoBehaviour
    {
        [Inject]
        public AssetDependency otherDependency;
    }
}
