using UnityEngine;

namespace Tests.SanejectLegacy.Runtime.NestedSerializable
{
    public class RequesterWithNestedSerializable : MonoBehaviour
    {
        [SerializeField]
        public NestedInjectedData nested;
    }
}