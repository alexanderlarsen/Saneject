using System;
using Plugins.Saneject.Legacy.Runtime.Attributes;
using UnityEngine;

namespace Tests.Legacy.Runtime.MethodInjection
{
    [Serializable]
    public class MyDeepNestedClass
    {
        [ReadOnly, SerializeField]
        private MyDependency[] deps;

        [Inject]
        private void Inject(MyDependency[] deps)
        {
            this.deps = deps;
        }
    }
}