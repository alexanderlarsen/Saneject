using Plugins.Saneject.Runtime.Attributes;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using UnityEngine;

namespace Tests.Saneject.Fixtures.Scripts.InjectionTargets
{
    public class FieldAndMethodTarget : MonoBehaviour
    {
        [Inject]
        public ComponentDependency fieldDependency;

        public ComponentDependency methodDependency;

        [Inject]
        private void Inject(ComponentDependency dependency)
        {
            methodDependency = dependency;
        }
    }
}
