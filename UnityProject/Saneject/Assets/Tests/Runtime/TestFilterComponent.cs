using Plugins.Saneject.Runtime.Attributes;
using UnityEngine;

namespace Tests.Runtime
{
    public class TestFilterComponent : MonoBehaviour
    {
        [Inject, SerializeField]
        public InjectableService componentTarget;

        [Inject, SerializeField]
        public Transform transformTarget;
        
        [Inject, SerializeField]
        public GameObject prefabTarget;

        [Inject, SerializeField]
        public TestScriptableObject scriptableObjectTarget;
    }
}