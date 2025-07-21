using Plugins.Saneject.Runtime.Attributes;
using UnityEngine;

namespace Tests.Runtime
{
    public partial class TestComponent : MonoBehaviour
    {
        [Inject, SerializeField]
        private InjectableService service;

        [Inject, SerializeInterface]
        private IInjectableService serviceInterface;
        
        public InjectableService Service => service;
        public IInjectableService ServiceInterface => serviceInterface;
    }
}