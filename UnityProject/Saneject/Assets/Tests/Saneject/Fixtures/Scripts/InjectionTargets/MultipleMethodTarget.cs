using Plugins.Saneject.Runtime.Attributes;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using UnityEngine;

namespace Tests.Saneject.Fixtures.Scripts.InjectionTargets
{
    public class MultipleMethodTarget : MonoBehaviour
    {
        public bool firstMethodCalled;
        public bool secondMethodCalled;
        public ComponentDependency firstDependency;
        public IDependency secondDependency;

        [Inject]
        private void InjectFirst(ComponentDependency dependency)
        {
            firstMethodCalled = true;
            firstDependency = dependency;
        }

        [Inject]
        private void InjectSecond(IDependency dependency)
        {
            secondMethodCalled = true;
            secondDependency = dependency;
        }
    }
}
