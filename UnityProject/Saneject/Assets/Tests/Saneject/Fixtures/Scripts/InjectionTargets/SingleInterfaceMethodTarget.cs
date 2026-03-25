using Plugins.Saneject.Runtime.Attributes;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using UnityEngine;

namespace Tests.Saneject.Fixtures.Scripts.InjectionTargets
{
    public class SingleInterfaceMethodTarget : MonoBehaviour
    {
        public IDependency dependency;

        [Inject]
        private void Inject(IDependency dependency)
        {
            this.dependency = dependency;
        }
    }
}
