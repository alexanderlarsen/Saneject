using System.Collections.Generic;
using Plugins.Saneject.Runtime.Attributes;
using UnityEngine;

namespace Tests.Runtime.MethodInjection
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