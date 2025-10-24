using Plugins.Saneject.Runtime.Attributes;
using Tests.Runtime.Component;
using UnityEngine;

namespace Tests.Runtime.NestedSerializable
{
    public class RootClass : MonoBehaviour
    {
        [Inject, SerializeField]
        private InjectableComponent injectableComponent;
        
        [SerializeField]
        private NestedClass nestedClass;
    }
}