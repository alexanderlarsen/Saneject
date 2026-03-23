using Plugins.Saneject.Runtime.Attributes;
using UnityEngine;

namespace Tests.Saneject.Runtime.Fixtures
{
    public class ConcreteRequester : MonoBehaviour
    {
        [Inject]
        public Dependency dependency;
    }
}