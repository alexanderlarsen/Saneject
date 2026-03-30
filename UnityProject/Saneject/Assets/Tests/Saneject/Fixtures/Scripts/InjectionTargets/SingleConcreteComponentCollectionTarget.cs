using Plugins.Saneject.Runtime.Attributes;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using UnityEngine;

namespace Tests.Saneject.Fixtures.Scripts.InjectionTargets
{
    public class SingleConcreteComponentCollectionTarget : MonoBehaviour
    {
        [Inject]
        public ComponentDependency[] dependencies;
    }
}
