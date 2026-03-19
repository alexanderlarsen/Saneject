using Plugins.SanejectLegacy.Runtime.Attributes;
using UnityEngine;

namespace Tests.SanejectLegacy.Runtime.MethodInjection
{
    public class MyClassWithPrivateInject : MonoBehaviour
    {
        [ReadOnly, SerializeField]
        public bool privateInjected, protectedInjected;

        [Inject]
        protected void InjectProtected(MyDependency dep)
        {
            protectedInjected = dep != null;
        }

        [Inject]
        private void InjectPrivate(MyDependency dep)
        {
            privateInjected = dep != null;
        }
    }
}