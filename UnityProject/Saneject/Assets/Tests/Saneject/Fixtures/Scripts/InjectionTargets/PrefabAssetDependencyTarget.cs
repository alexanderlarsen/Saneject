using Plugins.Saneject.Runtime.Attributes;
using UnityEngine;

namespace Tests.Saneject.Fixtures.Scripts.InjectionTargets
{
    public class PrefabAssetDependencyTarget : MonoBehaviour
    {
        [Inject, SerializeField]
        private GameObject dependency;

        public GameObject Dependency => dependency;
    }
}
