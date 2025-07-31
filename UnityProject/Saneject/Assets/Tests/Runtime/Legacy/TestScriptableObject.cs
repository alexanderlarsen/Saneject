using UnityEngine;

namespace Tests.Runtime.Legacy
{
    [CreateAssetMenu(fileName = "TestScriptableObject", menuName = "Saneject/Testing/TestScriptableObject", order = 0)]
    public class TestScriptableObject : ScriptableObject, ITestScriptableObject
    {
    }
}