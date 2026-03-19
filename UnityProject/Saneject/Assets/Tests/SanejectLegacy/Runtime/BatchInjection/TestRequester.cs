using Plugins.SanejectLegacy.Runtime.Attributes;
using UnityEngine;

namespace Tests.SanejectLegacy.Runtime.BatchInjection
{
    public class TestRequester : MonoBehaviour
    {
        [Inject, SerializeField]
        public TestDependency dependency;
    }
}