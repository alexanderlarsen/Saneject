using System.Linq;
using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Plugins.Saneject.Runtime.Scopes;
using Tests.Runtime;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Editor.BindingResolution.SpecialMethods
{
    public class FromResourcesTest
    {
        private TestScriptableObjectComponent testComponent;

        [SetUp]
        public void Setup()
        {
            LogAssert.ignoreFailingMessages = true;

            Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .ToList().ForEach(Object.DestroyImmediate);

            GameObject consumer = new("Consumer");
            testComponent = consumer.AddComponent<TestScriptableObjectComponent>();
            consumer.AddComponent<TestScope>();

            DependencyInjector.InjectSceneDependencies();
        }

        [Test]
        public void InjectsConcrete()
        {
            Assert.NotNull(testComponent.TestScriptableObject);
        }

        [Test]
        public void InjectsInterface()
        {
            Assert.NotNull(testComponent.TestScriptableObjectInterface);
        }

        public class TestScope : Scope
        {
            protected override void ConfigureBindings()
            {
                BindAsset<TestScriptableObject>()
                    .FromResources("Test/TestScriptableObject");

                BindAsset<ITestScriptableObject, TestScriptableObject>()
                    .FromResources("Test/TestScriptableObject");
            }
        }
    }
}