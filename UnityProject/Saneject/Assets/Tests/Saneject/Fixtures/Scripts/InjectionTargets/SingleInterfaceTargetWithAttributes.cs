using Plugins.Saneject.Runtime.Attributes;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using UnityEngine;

namespace Tests.Saneject.Fixtures.Scripts.InjectionTargets
{
    public partial class SingleInterfaceTargetWithAttributes : MonoBehaviour
    {
        [Inject, SerializeInterface]
        [Header("Field Header"), Tooltip("Field Tooltip")]
        public IDependency dependency;
    }
}
