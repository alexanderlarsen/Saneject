using Plugins.Saneject.Legacy.Runtime.Attributes;
using UnityEngine;

namespace Tests.Legacy.Runtime.MethodInjection
{
    public class MyClassWithPrivateInject : MonoBehaviour
    {
        [ReadOnly, SerializeField]
        public bool privateInjected, protectedInjected;

        [Inject]
        private void InjectPrivate(MyDependency dep)
        {
            privateInjected = dep != null;
        }

        [Inject]
        protected void InjectProtected(MyDependency dep)
        {
            protectedInjected = dep != null;
        }
    }
}