using System.Linq;
using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Plugins.Saneject.Runtime.Scopes;
using Tests.Runtime;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Editor.BindingResolution.InjectionTargetRelative
{
    public class FromInjectionTargetSiblingsTest
    {
        private Runtime.TestComponent testComponent;

        [SetUp]
        public void Setup()
        {
            LogAssert.ignoreFailingMessages = true;

            Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .ToList().ForEach(Object.DestroyImmediate);

            GameObject parent = new("Parent");

            GameObject sibling1 = new("Sibling1");
            sibling1.transform.SetParent(parent.transform);
            sibling1.AddComponent<InjectableService>();

            GameObject sibling2 = new("Sibling2");
            sibling2.transform.SetParent(parent.transform);

            GameObject targetObject = new("Target");
            targetObject.transform.SetParent(parent.transform);

            testComponent = targetObject.AddComponent<Runtime.TestComponent>();
            parent.AddComponent<TestScope>();

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
            public override void Configure()
            {
                Bind<InjectableService>().FromTargetSiblings();
                Bind<IInjectableService, InjectableService>().FromTargetSiblings();
            }
        }
    }
}