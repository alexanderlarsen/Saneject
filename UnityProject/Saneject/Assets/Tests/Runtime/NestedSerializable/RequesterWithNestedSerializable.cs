using UnityEngine;

namespace Tests.Runtime.NestedSerializable
{
    public class RequesterWithNestedSerializable : MonoBehaviour
    {
        [SerializeField]
        public NestedInjectedData nested;
    }
}