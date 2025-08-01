using Plugins.Saneject.Runtime.Attributes;
using UnityEngine;

namespace Tests.Runtime
{
    public partial class AssetRequester : MonoBehaviour
    {
        [Inject, SerializeField]
        public InjectableScriptableObject concreteAsset;

        [Inject, SerializeInterface]
        public IInjectable interfaceAsset;
    }
}