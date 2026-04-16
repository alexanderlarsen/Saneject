using Plugins.Saneject.Runtime.Attributes;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using UnityEngine;

namespace Tests.Saneject.Fixtures.Scripts.InjectionTargets
{
    public class NonPublicMethodTarget : MonoBehaviour
    {
        public bool protectedMethodCalled;
        public bool privateMethodCalled;

        [Inject]
        protected void InjectProtected(ComponentDependency dependency)
        {
            protectedMethodCalled = dependency != null;
        }

        [Inject]
        private void InjectPrivate(ComponentDependency dependency)
        {
            privateMethodCalled = dependency != null;
        }
    }
}
