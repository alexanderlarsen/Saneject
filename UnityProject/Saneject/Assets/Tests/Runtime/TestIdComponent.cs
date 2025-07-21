using Plugins.Saneject.Runtime.Attributes;
using UnityEngine;

namespace Tests.Runtime
{
    public partial class TestIdComponent : MonoBehaviour
    {
        [Inject("A"), SerializeField]
        private InjectableService injectableServiceA;

        [Inject("B"), SerializeField]
        private InjectableService injectableServiceB;

        [Inject("InterfaceA"), SerializeInterface]
        private IInjectableService injectableServiceInterfaceA;
        
        [Inject("InterfaceB"), SerializeInterface]
        private IInjectableService injectableServiceInterfaceB;
        
        public InjectableService InjectableServiceA => injectableServiceA;
        public InjectableService InjectableServiceB => injectableServiceB;
        public IInjectableService InjectableServiceInterfaceA => injectableServiceInterfaceA;
        public IInjectableService InjectableServiceInterfaceB => injectableServiceInterfaceB;
    }
}