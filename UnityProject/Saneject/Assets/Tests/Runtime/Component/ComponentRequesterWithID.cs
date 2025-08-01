using Plugins.Saneject.Runtime.Attributes;
using UnityEngine;

namespace Tests.Runtime.Component
{
    public partial class ComponentRequesterWithID : MonoBehaviour
    {
        [SerializeField, Inject("componentA")]
        public InjectableComponent concreteComponentA;

        [SerializeInterface, Inject("componentA")]
        public IInjectable interfaceComponentA;
        
        [SerializeField, Inject("componentB")]
        public InjectableComponent concreteComponentB;

        [SerializeInterface, Inject("componentB")]
        public IInjectable interfaceComponentB;
    }
}