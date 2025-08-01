using Plugins.Saneject.Runtime.Attributes;
using UnityEngine;

namespace Tests.Runtime.Asset
{
    public class GameObjectRequester : MonoBehaviour
    {
        [SerializeField, Inject]
        public GameObject gameObjectAsset;
    }

}