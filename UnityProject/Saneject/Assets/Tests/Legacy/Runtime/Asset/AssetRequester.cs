using Plugins.Saneject.Legacy.Runtime.Attributes;
using UnityEngine;

namespace Tests.Legacy.Runtime.Asset
{
    public partial class AssetRequester : MonoBehaviour
    {
        [Inject, SerializeField]
        public InjectableScriptableObject concreteAsset;

        [Inject, SerializeInterface]
        public IInjectable interfaceAsset;
    }
}