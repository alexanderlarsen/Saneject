using System.Collections.Generic;
using Plugins.Saneject.Runtime.Attributes;
using UnityEngine;

namespace Tests.Saneject.Fixtures.Scripts.InjectionTargets
{
    public class MultiPrefabAssetDependencyTarget : MonoBehaviour
    {
        [Inject]
        public GameObject[] array;

        [Inject]
        public List<GameObject> list;
    }
}
