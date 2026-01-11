using Plugins.Saneject.Legacy.Runtime.Attributes;
using UnityEngine;

namespace Tests.Legacy.Runtime.BatchInjection
{
    public class TestRequester : MonoBehaviour
    {
        [Inject, SerializeField]
        public TestDependency dependency;
    }
}