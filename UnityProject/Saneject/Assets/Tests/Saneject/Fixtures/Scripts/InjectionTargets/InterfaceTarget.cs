using Plugins.Saneject.Runtime.Attributes;
using UnityEngine;

namespace Tests.Saneject.Fixtures.Scripts
{
    public partial class InterfaceTarget : MonoBehaviour
    {
        [Inject, SerializeInterface]
        public IDependency dependency;
    }
}