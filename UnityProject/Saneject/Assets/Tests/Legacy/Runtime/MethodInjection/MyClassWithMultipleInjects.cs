using Plugins.Saneject.Legacy.Runtime.Attributes;
using UnityEngine;

namespace Tests.Legacy.Runtime.MethodInjection
{
    public class MyClassWithMultipleInjects : MonoBehaviour
    {
        [ReadOnly, SerializeField]
        public bool inject1Called, inject2Called;

        [ReadOnly, SerializeField]
        public MyDependency dependency1, dependency2;

        [Inject]
        private void InjectA(MyDependency dep)
        {
            inject1Called = true;
            dependency1 = dep;
        }

        [Inject]
        private void InjectB(IDependency dep)
        {
            inject2Called = true;
            dependency2 = dep as MyDependency;
        }
    }
}