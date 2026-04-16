using System.Collections.Generic;
using Plugins.Saneject.Runtime.Attributes;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using UnityEngine;

namespace Tests.Saneject.Fixtures.Scripts.InjectionTargets
{
    public class MultiInterfaceMethodTarget : MonoBehaviour
    {
        public IDependency[] array;
        public List<IDependency> list;

        [Inject]
        private void InjectArray(IDependency[] array)
        {
            this.array = array;
        }

        [Inject]
        private void InjectList(List<IDependency> list)
        {
            this.list = list;
        }
    }
}
