using Plugins.Saneject.Runtime.Attributes;
using UnityEngine;

namespace Tests.Saneject.Fixtures.Scripts
{
    public class ConcreteComponentTarget : MonoBehaviour
    {
        [Inject]
        public ComponentDependency dependency;
    }
}