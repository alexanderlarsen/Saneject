using Plugins.Saneject.Runtime.Attributes;
using UnityEngine;

namespace Tests.Runtime.MethodInjection
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