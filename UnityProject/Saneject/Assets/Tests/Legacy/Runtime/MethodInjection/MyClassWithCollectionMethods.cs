using System.Collections.Generic;
using Plugins.Saneject.Legacy.Runtime.Attributes;
using UnityEngine;

namespace Tests.Legacy.Runtime.MethodInjection
{
    public class MyClassWithCollectionMethods : MonoBehaviour
    {
        [ReadOnly, SerializeField]
        public MyDependency[] arrayDeps;

        [ReadOnly, SerializeField]
        public List<MyDependency> listDeps;

        [Inject]
        private void InjectCollections(
            MyDependency[] deps,
            List<MyDependency> depsList)
        {
            arrayDeps = deps;
            listDeps = depsList;
        }
    }
}