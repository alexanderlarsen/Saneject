using System.Collections.Generic;
using Plugins.Saneject.Runtime.Attributes;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using UnityEngine;

namespace Tests.Saneject.Fixtures.Scripts.InjectionTargets
{
    public class MultipleInterfaceTarget : MonoBehaviour
    {
        [Inject]
        public IDependency[] array;

        [Inject]
        public List<IDependency> list;
    }
}