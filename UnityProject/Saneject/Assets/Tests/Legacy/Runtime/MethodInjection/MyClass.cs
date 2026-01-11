using Plugins.Saneject.Legacy.Runtime.Attributes;
using UnityEngine;

namespace Tests.Legacy.Runtime.MethodInjection
{
    public partial class MyClass : MonoBehaviour
    {
        [SerializeField]
        private MyNestedClass nestedClass;

        [Inject, SerializeField]
        private MyDependency myDependency;

        [ReadOnly, SerializeField]
        private MyDependency myDependency2;

        [SerializeInterface, ReadOnly]
        private IDependency dependency;

        [Inject]
        private void Inject(
            MyDependency myDependency,
            IDependency dependency)
        {
            if (myDependency == null)
            {
                Debug.LogError("MyDependency is null");
                return;
            }

            myDependency2 = myDependency;
            this.dependency = dependency;
        }

        [Inject]
        void InjectTransform(Transform t)
        {
            Debug.Log(t.gameObject.name);
        }
    }
}