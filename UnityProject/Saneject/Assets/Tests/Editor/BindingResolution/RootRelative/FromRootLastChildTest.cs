using System.Linq;
using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Plugins.Saneject.Runtime.Scopes;
using Tests.Runtime;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Editor.BindingResolution.RootRelative
{
    public class FromRootLastChildTest
    {
        private Runtime.TestComponent testComponent;

        [SetUp]
        public void Setup()
        {
            LogAssert.ignoreFailingMessages = true;

            Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .ToList().ForEach(Object.DestroyImmediate);

            GameObject root = new("Root");

            GameObject firstChild = new("FirstChild");
            firstChild.transform.SetParent(root.transform);

            GameObject lastChild = new("LastChild");
            lastChild.transform.SetParent(root.transform);

            lastChild.AddComponent<InjectableService>(); // Only last child has the service
            testComponent = firstChild.AddComponent<Runtime.TestComponent>();
            firstChild.AddComponent<TestScope>();

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
                Bind<InjectableService>().FromRootLastChild();
                Bind<IInjectableService, InjectableService>().FromRootLastChild();
            }
        }
    }
}