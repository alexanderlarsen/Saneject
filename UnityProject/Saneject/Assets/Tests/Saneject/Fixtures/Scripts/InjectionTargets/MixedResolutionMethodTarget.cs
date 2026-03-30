using Plugins.Saneject.Runtime.Attributes;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using UnityEngine;

namespace Tests.Saneject.Fixtures.Scripts.InjectionTargets
{
    public class MixedResolutionMethodTarget : MonoBehaviour
    {
        public bool concreteMethodCalled;
        public bool mixedMethodCalled;
        public ComponentDependency concreteDependency;
        public ComponentDependency mixedConcreteDependency;
        public IDependency mixedInterfaceDependency;

        [Inject]
        private void InjectConcrete(ComponentDependency dependency)
        {
            concreteMethodCalled = true;
            concreteDependency = dependency;
        }

        [Inject]
        private void InjectMixed(
            ComponentDependency dependency,
            IDependency interfaceDependency)
        {
            mixedMethodCalled = true;
            mixedConcreteDependency = dependency;
            mixedInterfaceDependency = interfaceDependency;
        }
    }
}
