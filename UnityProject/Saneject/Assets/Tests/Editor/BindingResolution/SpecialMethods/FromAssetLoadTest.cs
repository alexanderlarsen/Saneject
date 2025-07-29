using System.Linq;
using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Plugins.Saneject.Runtime.Scopes;
using Tests.Runtime;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Editor.BindingResolution.SpecialMethods
{
    public class FromAssetLoadTest
    {
        private TestScriptableObjectComponent target;

        [SetUp]
        public void Setup()
        {
            LogAssert.ignoreFailingMessages = true;

            Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .ToList().ForEach(Object.DestroyImmediate);

            GameObject consumer = new("Consumer");
            target = consumer.AddComponent<TestScriptableObjectComponent>();
            consumer.AddComponent<TestScope>();

            DependencyInjector.InjectSceneDependencies();
        }

        [Test]
        public void InjectsConcrete()
        {
            Assert.NotNull(target.TestScriptableObject);
        }

        [Test]
        public void InjectsInterface()
        {
            Assert.NotNull(target.TestScriptableObjectInterface);
        }

        public class TestScope : Scope
        {
            protected override void ConfigureBindings()
            {
                const string assetPath = "Assets/Tests/Resources/Test/TestScriptableObject.asset";

                BindAsset<TestScriptableObject>()
                    .FromAssetLoad(assetPath);

                BindAsset<ITestScriptableObject, TestScriptableObject>()
                    .FromAssetLoad(assetPath);
            }
        }
    }
}