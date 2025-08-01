using Plugins.Saneject.Runtime.Attributes;
using UnityEngine;

namespace Tests.Runtime
{
    public partial class Requester : MonoBehaviour
    {
        [SerializeField, Inject]
        public InjectableComponent concreteComponent;

        [SerializeInterface, Inject]
        public IInjectable interfaceComponent;
    }
}