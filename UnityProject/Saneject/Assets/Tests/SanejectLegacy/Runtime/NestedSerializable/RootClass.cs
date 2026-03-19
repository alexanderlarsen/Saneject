using Plugins.SanejectLegacy.Runtime.Attributes;
using Tests.SanejectLegacy.Runtime.Component;
using UnityEngine;

namespace Tests.SanejectLegacy.Runtime.NestedSerializable
{
    public class RootClass : MonoBehaviour
    {
        [Inject, SerializeField]
        private InjectableComponent injectableComponent;

        [SerializeField]
        private NestedClass nestedClass;
    }
}