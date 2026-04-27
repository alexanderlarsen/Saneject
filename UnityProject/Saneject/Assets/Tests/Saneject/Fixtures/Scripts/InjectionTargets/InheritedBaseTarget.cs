using Plugins.Saneject.Runtime.Attributes;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using UnityEngine;

namespace Tests.Saneject.Fixtures.Scripts.InjectionTargets
{
    public abstract class InheritedBaseTarget : MonoBehaviour
    {
        [Inject, SerializeField]
        public ComponentDependency publicBaseDependency;

        [Inject, SerializeField]
        private ComponentDependency privateBaseDependency;

        public ComponentDependency PrivateBaseDependency => privateBaseDependency;

        public ComponentDependency PrivateBasePropertyValue => PrivateBaseProperty;

        [field: Inject, SerializeField]
        public ComponentDependency PublicBaseProperty { get; private set; }

        public ComponentDependency PublicBaseMethodDependency { get; private set; }
        public ComponentDependency PrivateBaseMethodDependency { get; private set; }

        [field: Inject, SerializeField]
        private ComponentDependency PrivateBaseProperty { get; set; }

        [Inject]
        public void PublicBaseMethod(ComponentDependency dependency)
        {
            PublicBaseMethodDependency = dependency;
        }

        [Inject]
        private void PrivateBaseMethod(ComponentDependency dependency)
        {
            PrivateBaseMethodDependency = dependency;
        }
    }
}