using System.Linq;
using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Plugins.Saneject.Runtime.Scopes;
using Tests.Runtime;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Editor.BindingResolution.CustomTargetRelative
{
    public class FromCustomTargetDescendantsTest
    {
        private TestComponent testComponent;

        [SetUp]
        public void Setup()
        {
            LogAssert.ignoreFailingMessages = true;

            Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .ToList().ForEach(Object.DestroyImmediate);

            GameObject target = new("Target");

            GameObject child = new("Child");
            child.transform.SetParent(target.transform);

            GameObject grandchild = new("Grandchild");
            grandchild.transform.SetParent(child.transform);

            grandchild.AddComponent<InjectableService>();

            GameObject consumer = new("Consumer");
            consumer.transform.SetParent(new GameObject("Root").transform);

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

            protected override void ConfigureBindings()
            {
                BindComponent<InjectableService>()
                    .FromDescendantsOf(target.transform);

                BindComponent<IInjectableService, InjectableService>()
                    .FromDescendantsOf(target.transform);
            }
        }
    }
}