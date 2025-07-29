using System.Linq;
using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Plugins.Saneject.Runtime.Scopes;
using Tests.Runtime;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Editor.BindingResolution.RootRelative
{
    public class FromRootFirstChildTest
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
            ;

            firstChild.AddComponent<InjectableService>();
            testComponent = secondChild.AddComponent<TestComponent>();
            secondChild.AddComponent<TestScope>();

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
                    .FromRootFirstChild();

                BindComponent<IInjectableService, InjectableService>()
                    .FromRootFirstChild();
            }
        }
    }
}