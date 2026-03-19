using Plugins.SanejectLegacy.Runtime.Attributes;
using UnityEngine;

namespace Tests.SanejectLegacy.Runtime.MethodInjection
{
    public class MyClassPartialInjection : MonoBehaviour
    {
        [ReadOnly, SerializeField]
        public bool methodCalled;

        [Inject]
        private void Inject(
            MyDependency dep,
            IDependency missingDep)
        {
            methodCalled = true;
        }
    }
}