using Plugins.Saneject.Runtime.Attributes;
using UnityEngine;

namespace Tests.Runtime.BatchInjection
{
    public class TestRequester : MonoBehaviour
    {
        [Inject, SerializeField]
        public TestDependency dependency;
    }
}