using System.Linq;
using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Plugins.Saneject.Runtime.Scopes;
using Tests.Runtime;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Editor.BindingResolution.InjectionTargetRelative
{
    public class FromTargetDescendantsTest
    {
        private Runtime.TestComponent testComponent;

        [SetUp]
        public void Setup()
        {
            LogAssert.ignoreFailingMessages = true;

            Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .ToList().ForEach(Object.DestroyImmediate);

            GameObject root = new("Root");

            GameObject parent = new("Target");
            parent.transform.SetParent(root.transform);

            GameObject child = new("Child");
            child.transform.SetParent(parent.transform);

            GameObject grandchild = new("Grandchild");
            grandchild.transform.SetParent(child.transform);

            grandchild.AddComponent<InjectableService>();
            testComponent = parent.AddComponent<Runtime.TestComponent>();
            root.AddComponent<TestScope>();

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
                Bind<InjectableService>().FromTargetDescendants();
                Bind<IInjectableService, InjectableService>().FromTargetDescendants();
            }
        }
    }
}