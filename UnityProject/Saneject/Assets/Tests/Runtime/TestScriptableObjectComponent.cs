using Plugins.Saneject.Runtime.Attributes;
using UnityEngine;

namespace Tests.Runtime
{
    public partial class TestScriptableObjectComponent : MonoBehaviour
    {
        [Inject, SerializeField]
        private TestScriptableObject testScriptableObject;
        
        [Inject, SerializeInterface]
        private ITestScriptableObject testScriptableObjectInterface;
        
        public TestScriptableObject TestScriptableObject => testScriptableObject;
        public ITestScriptableObject TestScriptableObjectInterface => testScriptableObjectInterface;
    }
}