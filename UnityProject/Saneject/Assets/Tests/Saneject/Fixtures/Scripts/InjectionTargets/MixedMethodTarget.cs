using System.Collections.Generic;
using Plugins.Saneject.Runtime.Attributes;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using UnityEngine;

namespace Tests.Saneject.Fixtures.Scripts.InjectionTargets
{
    public class MixedMethodTarget : MonoBehaviour
    {
        public ComponentDependency singleConcreteDependency;
        public IDependency singleInterfaceDependency;
        public ComponentDependency[] concreteArray;
        public List<IDependency> interfaceList;
        public ComponentDependency mixedConcreteDependency;
        public List<ComponentDependency> mixedConcreteList;
        public IDependency mixedInterfaceDependency;

        [Inject]
        private void InjectSingles(
            ComponentDependency singleConcreteDependency,
            IDependency singleInterfaceDependency)
        {
            this.singleConcreteDependency = singleConcreteDependency;
            this.singleInterfaceDependency = singleInterfaceDependency;
        }

        [Inject]
        private void InjectCollections(
            ComponentDependency[] concreteArray,
            List<IDependency> interfaceList)
        {
            this.concreteArray = concreteArray;
            this.interfaceList = interfaceList;
        }

        [Inject]
        private void InjectMixed(
            ComponentDependency mixedConcreteDependency,
            List<ComponentDependency> mixedConcreteList,
            IDependency mixedInterfaceDependency)
        {
            this.mixedConcreteDependency = mixedConcreteDependency;
            this.mixedConcreteList = mixedConcreteList;
            this.mixedInterfaceDependency = mixedInterfaceDependency;
        }
    }
}
