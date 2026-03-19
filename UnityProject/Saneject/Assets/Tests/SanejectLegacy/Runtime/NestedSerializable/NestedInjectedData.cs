using System;
using Plugins.SanejectLegacy.Runtime.Attributes;
using Tests.SanejectLegacy.Runtime.Component;
using UnityEngine;

namespace Tests.SanejectLegacy.Runtime.NestedSerializable
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