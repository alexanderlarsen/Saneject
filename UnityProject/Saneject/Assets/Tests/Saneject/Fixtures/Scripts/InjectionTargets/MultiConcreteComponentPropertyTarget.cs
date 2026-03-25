using System.Collections.Generic;
using Plugins.Saneject.Runtime.Attributes;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using UnityEngine;

namespace Tests.Saneject.Fixtures.Scripts.InjectionTargets
{
    public class MultiConcreteComponentPropertyTarget : MonoBehaviour
    {
        [field: Inject, SerializeField]
        public ComponentDependency[] Array { get; private set; }

        [field: Inject, SerializeField]
        public List<ComponentDependency> List { get; private set; }
    }
}
