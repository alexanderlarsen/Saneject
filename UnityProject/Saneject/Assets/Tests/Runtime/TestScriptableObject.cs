using UnityEngine;

namespace Tests.Runtime
{
    [CreateAssetMenu(fileName = "TestScriptableObject", menuName = "Saneject/Testing/TestScriptableObject", order = 0)]
    public class TestScriptableObject : ScriptableObject, ITestScriptableObject
    {
    }
}