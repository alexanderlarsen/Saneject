using Plugins.Saneject.Runtime.Attributes;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using UnityEngine;

namespace Tests.Saneject.Fixtures.Scripts.InjectionTargets
{
    public partial class SuppressedSingleInterfaceTarget : MonoBehaviour
    {
        [Inject(true), SerializeInterface]
        public IDependency dependency;
    }
}
