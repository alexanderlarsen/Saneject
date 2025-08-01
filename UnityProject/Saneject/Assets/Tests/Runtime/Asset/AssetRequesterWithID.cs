using Plugins.Saneject.Runtime.Attributes;
using UnityEngine;

namespace Tests.Runtime.Asset
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