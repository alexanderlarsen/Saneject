using Plugins.SanejectLegacy.Runtime.Attributes;
using UnityEngine;

namespace Tests.SanejectLegacy.Runtime.Asset
{
    public partial class AssetRequesterB : MonoBehaviour
    {
        [Inject, SerializeField]
        public InjectableScriptableObject concreteAsset;

        [Inject, SerializeInterface]
        public IInjectable interfaceAsset;
    }
}