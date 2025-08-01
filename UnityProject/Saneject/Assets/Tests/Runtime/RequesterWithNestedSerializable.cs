using UnityEngine;

namespace Tests.Runtime
{
    public class RequesterWithNestedSerializable : MonoBehaviour
    {
        [SerializeField]
        public NestedInjectedData nested;
    }
}