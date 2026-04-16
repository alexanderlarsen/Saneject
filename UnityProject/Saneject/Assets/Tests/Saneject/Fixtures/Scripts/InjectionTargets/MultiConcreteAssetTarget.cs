using System.Collections.Generic;
using Plugins.Saneject.Runtime.Attributes;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using UnityEngine;

namespace Tests.Saneject.Fixtures.Scripts.InjectionTargets
{
    public class MultiConcreteAssetTarget : MonoBehaviour
    {
        [Inject]
        public AssetDependency[] array;

        [Inject]
        public List<AssetDependency> list;
    }
}