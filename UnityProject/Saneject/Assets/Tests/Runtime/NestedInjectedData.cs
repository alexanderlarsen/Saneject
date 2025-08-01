using System;
using Plugins.Saneject.Runtime.Attributes;
using UnityEngine;

namespace Tests.Runtime
{
    [Serializable]
    public partial class NestedInjectedData
    {
        [SerializeField, Inject]
        public InjectableComponent concrete;

        [SerializeInterface, Inject]
        public IInjectable interfaceComponent;
    }
}