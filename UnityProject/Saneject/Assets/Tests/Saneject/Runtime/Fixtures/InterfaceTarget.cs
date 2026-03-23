using Plugins.Saneject.Runtime.Attributes;
using UnityEngine;

namespace Tests.Saneject.Runtime.Fixtures
{
    public partial class InterfaceTarget : MonoBehaviour
    {
        [Inject, SerializeInterface]
        public IDependency dependency;
    }
}