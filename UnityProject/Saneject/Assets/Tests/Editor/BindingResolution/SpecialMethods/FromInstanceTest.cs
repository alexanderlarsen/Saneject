using System.Linq;
using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Plugins.Saneject.Runtime.Scopes;
using Tests.Runtime;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Editor.BindingResolution.SpecialMethods
{
    public class FromInstanceTest
    {
        private Runtime.TestComponent testComponent;

        [SetUp]
        public void Setup()
        {
            LogAssert.ignoreFailingMessages = true;

            Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .ToList().ForEach(Object.DestroyImmediate);

            GameObject instanceHolder = new("InstanceHolder");
            InjectableService instance = instanceHolder.AddComponent<InjectableService>();

            GameObject consumer = new("ConsumerObject");
            testComponent = consumer.AddComponent<Runtime.TestComponent>();

            TestScope scope = consumer.AddComponent<TestScope>();
            scope.instance = instance;

            DependencyInjector.InjectSceneDependencies();
        }

        [Test]
        public void InjectsConcrete()
        {
            Assert.NotNull(testComponent.Service);
        }

        [Test]
        public void InjectsInterface()
        {
            Assert.NotNull(testComponent.ServiceInterface);
        }

        public class TestScope : Scope
        {
            public InjectableService instance;

            public override void Configure()
            {
                Bind<InjectableService>().FromInstance(instance);
                Bind<IInjectableService, InjectableService>().FromInstance(instance);
            }
        }
    }
}