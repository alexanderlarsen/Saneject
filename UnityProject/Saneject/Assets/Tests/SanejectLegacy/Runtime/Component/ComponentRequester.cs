using Plugins.SanejectLegacy.Runtime.Attributes;
using UnityEngine;

namespace Tests.SanejectLegacy.Runtime.Component
{
    public partial class ComponentRequester : MonoBehaviour
    {
        [SerializeField, Inject]
        public InjectableComponent concreteComponent;

        [SerializeInterface, Inject]
        public IInjectable interfaceComponent;
    }
}