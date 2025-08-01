using Plugins.Saneject.Runtime.Attributes;
using UnityEngine;

namespace Tests.Runtime.Component
{
    public partial class ComponentRequesterB : MonoBehaviour
    {
        [SerializeField, Inject]
        public InjectableComponent concreteComponent;

        [SerializeInterface, Inject]
        public IInjectable interfaceComponent;
    }
}