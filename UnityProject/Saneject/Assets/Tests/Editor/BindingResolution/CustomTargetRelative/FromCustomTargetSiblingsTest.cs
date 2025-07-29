using System.Linq;
using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Plugins.Saneject.Runtime.Scopes;
using Tests.Runtime;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Editor.BindingResolution.CustomTargetRelative
{
    public class FromCustomTargetSiblingsTest
    {
        private TestComponent testComponent;

        [SetUp]
        public void Setup()
        {
            LogAssert.ignoreFailingMessages = true;

            Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .ToList().ForEach(Object.DestroyImmediate);

            GameObject parent = new("Parent");

            GameObject sibling1 = new("Sibling1");
            sibling1.transform.SetParent(parent.transform);

            GameObject sibling2 = new("Sibling2");
            sibling2.transform.SetParent(parent.transform);
            sibling2.AddComponent<InjectableService>();

            GameObject target = new("Target");
            target.transform.SetParent(parent.transform);

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
                    .FromSiblingsOf(target.transform);

                BindComponent<IInjectableService, InjectableService>()
                    .FromSiblingsOf(target.transform);
            }
        }
    }
}