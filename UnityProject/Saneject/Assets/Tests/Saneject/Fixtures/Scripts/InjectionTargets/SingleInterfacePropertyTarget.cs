using Plugins.Saneject.Runtime.Attributes;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using UnityEngine;

namespace Tests.Saneject.Fixtures.Scripts.InjectionTargets
{
    public partial class SingleInterfacePropertyTarget : MonoBehaviour
    {
        [field: Inject, SerializeInterface]
        public IDependency Dependency { get; private set; }
    }
}
