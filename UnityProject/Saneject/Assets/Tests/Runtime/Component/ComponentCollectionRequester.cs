using System.Collections.Generic;
using Plugins.Saneject.Runtime.Attributes;
using UnityEngine;

namespace Tests.Runtime.Component
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