using UnityEngine;

namespace Tests.Runtime
{
    [CreateAssetMenu(fileName = "InjectableScriptableObject", menuName = "Saneject/Testing/InjectableScriptableObject", order = 0)]
    public class InjectableScriptableObject : ScriptableObject, IInjectable
    {
    }
}