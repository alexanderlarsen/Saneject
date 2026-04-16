using System.Collections.Generic;
using Plugins.Saneject.Runtime.Attributes;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using UnityEngine;

namespace Tests.Saneject.Fixtures.Scripts.InjectionTargets
{
    public partial class MultiInterfaceTarget : MonoBehaviour
    {
        [Inject, SerializeInterface]
        public IDependency[] array;

        [Inject, SerializeInterface]
        public List<IDependency> list;
    }
}