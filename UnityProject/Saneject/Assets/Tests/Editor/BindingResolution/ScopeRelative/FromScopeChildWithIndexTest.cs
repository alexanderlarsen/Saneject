using System.Linq;
using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Plugins.Saneject.Runtime.Scopes;
using Tests.Runtime;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Editor.BindingResolution.ScopeRelative
{
    public class FromScopeChildWithIndexTest
    {
        private Runtime.TestComponent testComponent;

        [SetUp]
        public void Setup()
        {
            LogAssert.ignoreFailingMessages = true;

            Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList().ForEach(Object.DestroyImmediate);

            GameObject root = new("Root");

            GameObject firstChild = new("FirstChild");
            firstChild.transform.SetParent(root.transform);

            GameObject secondChild = new("SecondChild");
            secondChild.transform.SetParent(root.transform);

            GameObject thirdChild = new("ThirdChild");
            thirdChild.transform.SetParent(root.transform);

            root.AddComponent<TestScope>(); // Scope on parent
            thirdChild.AddComponent<InjectableService>();
            testComponent = root.AddComponent<Runtime.TestComponent>(); // Target on scope itself

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
                Bind<InjectableService>().FromScopeChildWithIndex(2);
                Bind<IInjectableService, InjectableService>().FromScopeChildWithIndex(2);
            }
        }
    }
}