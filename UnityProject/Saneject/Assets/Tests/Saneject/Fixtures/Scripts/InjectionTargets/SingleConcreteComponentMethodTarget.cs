using Plugins.Saneject.Runtime.Attributes;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using UnityEngine;

namespace Tests.Saneject.Fixtures.Scripts.InjectionTargets
{
    public class SingleConcreteComponentMethodTarget : MonoBehaviour
    {
        public ComponentDependency dependency;

        [Inject]
        private void Inject(ComponentDependency dependency)
        {
            this.dependency = dependency;
        }
    }
}
