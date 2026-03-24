using Plugins.Saneject.Runtime.Attributes;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using UnityEngine;

namespace Tests.Saneject.Fixtures.Scripts.InjectionTargets
{
    public class SingleConcreteComponentTargetWithID : MonoBehaviour
    {
        [Inject("qualified")]
        public ComponentDependency dependency;
    }
}
