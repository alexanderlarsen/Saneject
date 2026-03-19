using System;
using Plugins.SanejectLegacy.Runtime.Attributes;
using UnityEngine;

namespace Tests.SanejectLegacy.Runtime.MethodInjection
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