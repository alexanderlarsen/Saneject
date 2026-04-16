using System.Collections.Generic;
using Plugins.Saneject.Runtime.Attributes;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using UnityEngine;

namespace Tests.Saneject.Fixtures.Scripts.InjectionTargets
{
    public class MultiConcreteComponentMethodTarget : MonoBehaviour
    {
        public ComponentDependency[] array;
        public List<ComponentDependency> list;

        [Inject]
        private void InjectArray(ComponentDependency[] array)
        {
            this.array = array;
        }

        [Inject]
        private void InjectList(List<ComponentDependency> list)
        {
            this.list = list;
        }
    }
}
