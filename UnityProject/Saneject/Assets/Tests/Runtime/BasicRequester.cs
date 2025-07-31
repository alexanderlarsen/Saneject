using Plugins.Saneject.Runtime.Attributes;
using UnityEngine;

namespace Tests.Runtime
{
    public partial class BasicRequester : MonoBehaviour
    {
        [SerializeField, Inject]
        public InjectableComponent concreteComponent;
        
        [SerializeInterface, Inject]
        public IInjectable interfaceComponent;

        public void Nullify()
        {
            concreteComponent = null;
            interfaceComponent = null;
            __interfaceComponent = null;
        }
    }
}