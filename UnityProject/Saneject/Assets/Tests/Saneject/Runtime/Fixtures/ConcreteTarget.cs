using Plugins.Saneject.Runtime.Attributes;
using UnityEngine;

namespace Tests.Saneject.Runtime.Fixtures
{
    public class ConcreteTarget : MonoBehaviour
    {
        [Inject]
        public Dependency dependency;
    }
}