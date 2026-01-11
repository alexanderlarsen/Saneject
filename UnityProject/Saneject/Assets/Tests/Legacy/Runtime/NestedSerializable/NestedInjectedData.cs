using System;
using Plugins.Saneject.Legacy.Runtime.Attributes;
using Tests.Legacy.Runtime.Component;
using UnityEngine;

namespace Tests.Legacy.Runtime.NestedSerializable
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