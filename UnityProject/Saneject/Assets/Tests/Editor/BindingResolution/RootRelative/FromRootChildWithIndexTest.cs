using System.Linq;
using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Plugins.Saneject.Runtime.Scopes;
using Tests.Runtime;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Editor.BindingResolution.RootRelative
{
    public class FromRootChildWithIndexTest
    {
        private TestComponent testComponent;

        [SetUp]
        public void Setup()
        {
            LogAssert.ignoreFailingMessages = true;

            Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .ToList().ForEach(Object.DestroyImmediate);

            GameObject root = new("Root");

            GameObject firstChild = new("FirstChild");
            firstChild.transform.SetParent(root.transform);

            GameObject secondChild = new("SecondChild");
            secondChild.transform.SetParent(root.transform);

            GameObject thirdChild = new("ThirdChild");
            thirdChild.transform.SetParent(root.transform);

            thirdChild.AddComponent<InjectableService>();
            testComponent = firstChild.AddComponent<TestComponent>();
            firstChild.AddComponent<TestScope>(); // Scope below root

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
            protected override void ConfigureBindings()
            {
                BindComponent<InjectableService>()
                    .FromRootChildWithIndex(2);

                BindComponent<IInjectableService, InjectableService>()
                    .FromRootChildWithIndex(2);
            }
        }
    }
}