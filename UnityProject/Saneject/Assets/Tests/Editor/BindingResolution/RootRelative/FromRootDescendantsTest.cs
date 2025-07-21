using System.Linq;
using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Plugins.Saneject.Runtime.Scopes;
using Tests.Runtime;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Editor.BindingResolution.RootRelative
{
    public class FromRootDescendantsTest
    {
        private Runtime.TestComponent testComponent;

        [SetUp]
        public void Setup()
        {
            LogAssert.ignoreFailingMessages = true;

            Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .ToList().ForEach(Object.DestroyImmediate);

            GameObject root = new("Root");
            GameObject child = new("Child");
            child.transform.SetParent(root.transform);

            GameObject grandchild = new("Grandchild");
            grandchild.transform.SetParent(child.transform);

            grandchild.AddComponent<InjectableService>(); // binding deep in descendants
            testComponent = child.AddComponent<Runtime.TestComponent>();
            child.AddComponent<TestScope>(); // scope is in the middle of the tree

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
                Bind<InjectableService>().FromRootDescendants();
                Bind<IInjectableService, InjectableService>().FromRootDescendants();
            }
        }
    }
}