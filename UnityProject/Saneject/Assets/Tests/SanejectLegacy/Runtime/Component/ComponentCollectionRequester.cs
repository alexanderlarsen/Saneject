using System.Collections.Generic;
using Plugins.SanejectLegacy.Runtime.Attributes;
using UnityEngine;

namespace Tests.SanejectLegacy.Runtime.Component
{
    public partial class ComponentCollectionRequester : MonoBehaviour
    {
        [SerializeField, Inject]
        public InjectableComponent[] servicesConcreteArray;

        [SerializeField, Inject]
        public List<InjectableComponent> servicesConcreteList;

        [SerializeInterface, Inject]
        public IInjectable[] servicesInterfaceArray;

        [SerializeInterface, Inject]
        public List<IInjectable> servicesInterfaceList;

        [SerializeField, Inject]
        public InjectableComponent serviceConcreteSingle;

        [SerializeInterface, Inject]
        public IInjectable serviceInterfaceSingle;
    }
}