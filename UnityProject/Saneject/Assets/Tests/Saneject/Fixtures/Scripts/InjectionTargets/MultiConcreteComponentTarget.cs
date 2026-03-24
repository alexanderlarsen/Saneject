using System.Collections.Generic;
using Plugins.Saneject.Runtime.Attributes;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using UnityEngine;

namespace Tests.Saneject.Fixtures.Scripts.InjectionTargets
{
    public class MultiConcreteComponentTarget : MonoBehaviour
    {
        [Inject]
        public ComponentDependency[] array;

        [Inject]
        public List<ComponentDependency> list;
    }
}