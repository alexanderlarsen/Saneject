using System;
using Plugins.Saneject.Legacy.Runtime.Attributes;
using Tests.Legacy.Runtime.Component;
using UnityEngine;

namespace Tests.Legacy.Runtime.NestedSerializable
{
    [Serializable]
    public class DeepNestedClass
    {
        [Inject, SerializeField]
        private InjectableComponent injectableComponent;
    }
}