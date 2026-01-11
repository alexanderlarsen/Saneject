using Plugins.Saneject.Legacy.Runtime.Attributes;
using Tests.Legacy.Runtime.Component;
using UnityEngine;

namespace Tests.Legacy.Runtime.NestedSerializable
{
    public class RootClass : MonoBehaviour
    {
        [Inject, SerializeField]
        private InjectableComponent injectableComponent;
        
        [SerializeField]
        private NestedClass nestedClass;
    }
}