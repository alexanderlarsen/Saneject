using Plugins.Saneject.Runtime.Attributes;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using UnityEngine;

namespace Tests.Saneject.Fixtures.Scripts.InjectionTargets
{
    public class PartialMethodTarget : MonoBehaviour
    {
        public bool methodCalled;
        public ComponentDependency dependency;
        public IDependency interfaceDependency;

        [Inject]
        private void Inject(
            ComponentDependency dependency,
            IDependency interfaceDependency)
        {
            methodCalled = true;
            this.dependency = dependency;
            this.interfaceDependency = interfaceDependency;
        }
    }
}
