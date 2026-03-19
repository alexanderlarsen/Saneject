using UnityEngine;

namespace Tests.SanejectLegacy.Runtime.Asset
{
    [CreateAssetMenu(fileName = "InjectableScriptableObject", menuName = "Saneject/Testing/InjectableScriptableObject", order = 0)]
    public class InjectableScriptableObject : ScriptableObject, IInjectable
    {
    }
}