using Plugins.SanejectLegacy.Runtime.Attributes;
using UnityEngine;

namespace Tests.SanejectLegacy.Runtime.Component
{
    public partial class ComponentRequesterB : MonoBehaviour
    {
        [SerializeField, Inject]
        public InjectableComponent concreteComponent;

        [SerializeInterface, Inject]
        public IInjectable interfaceComponent;
    }
}