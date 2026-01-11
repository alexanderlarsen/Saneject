using UnityEngine;

namespace Tests.Legacy.Runtime.NestedSerializable
{
    public class RequesterWithNestedSerializable : MonoBehaviour
    {
        [SerializeField]
        public NestedInjectedData nested;
    }
}