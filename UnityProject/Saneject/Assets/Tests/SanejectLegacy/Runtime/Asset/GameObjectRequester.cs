using Plugins.SanejectLegacy.Runtime.Attributes;
using UnityEngine;

namespace Tests.SanejectLegacy.Runtime.Asset
{
    public class GameObjectRequester : MonoBehaviour
    {
        [SerializeField, Inject]
        public GameObject gameObjectAsset;
    }
}