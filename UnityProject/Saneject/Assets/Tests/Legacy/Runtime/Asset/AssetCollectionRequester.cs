using System.Collections.Generic;
using Plugins.Saneject.Legacy.Runtime.Attributes;
using UnityEngine;

namespace Tests.Legacy.Runtime.Asset
{
    public partial class AssetCollectionRequester : MonoBehaviour
    {
        [SerializeField, Inject]
        public InjectableScriptableObject[] assetsConcreteArray;

        [SerializeField, Inject]
        public List<InjectableScriptableObject> assetsConcreteList;

        [SerializeInterface, Inject]
        public IInjectable[] assetsInterfaceArray;

        [SerializeInterface, Inject]
        public List<IInjectable> assetsInterfaceList;

        [SerializeField, Inject]
        public InjectableScriptableObject assetConcreteSingle;

        [SerializeInterface, Inject]
        public IInjectable assetInterfaceSingle;
    }
}