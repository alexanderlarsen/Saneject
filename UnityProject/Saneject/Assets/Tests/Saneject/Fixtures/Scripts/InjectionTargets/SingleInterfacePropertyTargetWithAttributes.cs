using Plugins.Saneject.Runtime.Attributes;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using UnityEngine;

namespace Tests.Saneject.Fixtures.Scripts.InjectionTargets
{
    public partial class SingleInterfacePropertyTargetWithAttributes : MonoBehaviour
    {
        [field: Inject, SerializeInterface]
        [field: Header("Property Header"), Tooltip("Property Tooltip")]
        public IDependency Dependency { get; private set; }
    }
}
