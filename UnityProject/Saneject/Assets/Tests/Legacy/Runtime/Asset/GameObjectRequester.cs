using Plugins.Saneject.Legacy.Runtime.Attributes;
using UnityEngine;

namespace Tests.Legacy.Runtime.Asset
{
    public class GameObjectRequester : MonoBehaviour
    {
        [SerializeField, Inject]
        public GameObject gameObjectAsset;
    }

}