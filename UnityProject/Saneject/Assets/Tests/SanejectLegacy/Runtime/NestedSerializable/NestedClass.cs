using System;
using Plugins.SanejectLegacy.Runtime.Attributes;
using Tests.SanejectLegacy.Runtime.Component;
using UnityEngine;

namespace Tests.SanejectLegacy.Runtime.NestedSerializable
{
    [Serializable]
    public class NestedClass
    {
        [Inject, SerializeField]
        private InjectableComponent injectableComponent;

        [SerializeField]
        private DeepNestedClass deepNestedClass;
    }
}