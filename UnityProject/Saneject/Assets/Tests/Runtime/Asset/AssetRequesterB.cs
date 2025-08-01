using Plugins.Saneject.Runtime.Attributes;
using UnityEngine;

namespace Tests.Runtime.Asset
{
    public partial class AssetRequesterB : MonoBehaviour
    {
        [Inject, SerializeField]
        public InjectableScriptableObject concreteAsset;

        [Inject, SerializeInterface]
        public IInjectable interfaceAsset;
    }
}