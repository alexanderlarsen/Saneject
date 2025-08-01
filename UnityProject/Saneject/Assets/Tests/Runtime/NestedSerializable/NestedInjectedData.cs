using System;
using Plugins.Saneject.Runtime.Attributes;
using Tests.Runtime.Component;
using UnityEngine;

namespace Tests.Runtime.NestedSerializable
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