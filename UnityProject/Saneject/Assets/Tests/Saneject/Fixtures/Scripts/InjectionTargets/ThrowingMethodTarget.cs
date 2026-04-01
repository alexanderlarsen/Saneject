using Plugins.Saneject.Runtime.Attributes;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using UnityEngine;

namespace Tests.Saneject.Fixtures.Scripts.InjectionTargets
{
    public class ThrowingMethodTarget : MonoBehaviour
    {
        public bool methodCalled;
        public ComponentDependency dependency;

        [Inject]
        private void Inject(ComponentDependency dependency)
        {
            methodCalled = true;
            this.dependency = dependency;
            throw new System.InvalidOperationException("Inject method exception");
        }
    }
}
