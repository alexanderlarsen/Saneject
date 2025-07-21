using System.Linq;
using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Plugins.Saneject.Runtime.Scopes;
using Tests.Runtime;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Editor.BindingResolution.CustomTargetRelative
{
    public class FromCustomTargetParentTest
    {
        private TestComponent testComponent;

        [SetUp]
        public void Setup()
        {
            LogAssert.ignoreFailingMessages = true;

            Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .ToList().ForEach(Object.DestroyImmediate);

            GameObject root = new("Root");
            GameObject target = new("Target");
            target.transform.SetParent(root.transform);

            GameObject consumer = new("Consumer");
            consumer.transform.SetParent(target.transform);

            root.AddComponent<InjectableService>();
            testComponent = consumer.AddComponent<TestComponent>();

            TestScope testScope = consumer.AddComponent<TestScope>();
            testScope.target = target;

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
            public GameObject target;

            public override void Configure()
            {
                Bind<InjectableService>().FromParentOf(target.transform);
                Bind<IInjectableService, InjectableService>().FromParentOf(target.transform);
            }
        }
    }
}