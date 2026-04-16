using Plugins.Saneject.Runtime.Attributes;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using UnityEngine;

namespace Tests.Saneject.Fixtures.Scripts.InjectionTargets
{
    public class SingleConcreteComponentPropertyTarget : MonoBehaviour
    {
        [field: Inject, SerializeField]
        public ComponentDependency Dependency { get; private set; }
    }
}
