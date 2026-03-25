using System.Collections.Generic;
using Plugins.Saneject.Runtime.Attributes;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using UnityEngine;

namespace Tests.Saneject.Fixtures.Scripts.InjectionTargets
{
    public partial class MultiInterfacePropertyTarget : MonoBehaviour
    {
        [field: Inject, SerializeInterface]
        public IDependency[] Array { get; private set; }

        [field: Inject, SerializeInterface]
        public List<IDependency> List { get; private set; }
    }
}
