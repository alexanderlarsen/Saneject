using System;
using Plugins.Saneject.Runtime.Attributes;
using UnityEngine;

namespace Tests.Runtime.MethodInjection
{
    [Serializable]
    public class MyNestedClass
    {
        [ReadOnly, SerializeField]
        private MyDependency[] myDependencies;

        [Inject]
        public void Inject(MyDependency[] dependencies)
        {
            if (dependencies == null)
            {
                Debug.LogError("MyDependency is null");
                return;
            }

            myDependencies = dependencies;
        }
    }
}