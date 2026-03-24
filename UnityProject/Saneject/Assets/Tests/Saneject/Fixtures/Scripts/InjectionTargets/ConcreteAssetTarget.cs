using Plugins.Saneject.Runtime.Attributes;
using UnityEngine;

namespace Tests.Saneject.Fixtures.Scripts
{
    public class ConcreteAssetTarget : MonoBehaviour
    {
        [Inject]
        public AssetDependency dependency;
    }
}