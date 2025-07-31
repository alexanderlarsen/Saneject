using System.Collections.Generic;
using Plugins.Saneject.Runtime.Attributes;
using UnityEngine;

namespace Tests.Runtime.Legacy
{
    public partial class CollectionTestComponent : MonoBehaviour
    {
        [SerializeField, Inject]
        public InjectableService[] servicesConcreteArray;

        [SerializeField, Inject]
        public List<InjectableService> servicesConcreteList;

        [SerializeInterface, Inject]
        public IInjectableService[] servicesInterfaceArray;

        [SerializeInterface, Inject]
        public List<IInjectableService> servicesInterfaceList;

        [SerializeField, Inject]
        public InjectableService serviceConcreteSingle;
        
        [SerializeInterface, Inject]
        public IInjectableService serviceInterfaceSingle;
    }
}