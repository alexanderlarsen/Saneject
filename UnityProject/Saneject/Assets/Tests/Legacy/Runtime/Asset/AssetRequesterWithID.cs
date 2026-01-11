using Plugins.Saneject.Legacy.Runtime.Attributes;
using UnityEngine;

namespace Tests.Legacy.Runtime.Asset
{
    public partial class AssetRequesterWithID : MonoBehaviour
    {
        [SerializeField, Inject("componentA")]
        public InjectableScriptableObject concreteComponentA;

        [SerializeInterface, Inject("componentA")]
        public IInjectable interfaceComponentA;

        [SerializeField, Inject("componentB")]
        public InjectableScriptableObject concreteComponentB;

        [SerializeInterface, Inject("componentB")]
        public IInjectable interfaceComponentB;
    }
}