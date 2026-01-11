using Plugins.Saneject.Legacy.Runtime.Attributes;
using UnityEngine;

namespace Tests.Legacy.Runtime.Component
{
    public partial class ComponentRequesterB : MonoBehaviour
    {
        [SerializeField, Inject]
        public InjectableComponent concreteComponent;

        [SerializeInterface, Inject]
        public IInjectable interfaceComponent;
    }
}