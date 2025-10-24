using System;
using Plugins.Saneject.Runtime.Attributes;
using Tests.Runtime.Component;
using UnityEngine;

namespace Tests.Runtime.NestedSerializable
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