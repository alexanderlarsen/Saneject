using Plugins.Saneject.Runtime.Attributes;
using UnityEngine;

namespace Tests.Saneject.Runtime.Fixtures
{
    public partial class InterfaceRequester : MonoBehaviour
    {
        [Inject, SerializeInterface]
        public IDependency dependency;
    }
}