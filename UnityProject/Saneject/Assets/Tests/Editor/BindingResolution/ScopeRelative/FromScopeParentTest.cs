using System.Linq;
using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Plugins.Saneject.Runtime.Scopes;
using Tests.Runtime;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Editor.BindingResolution.ScopeRelative
{
    public class FromScopeParentTest
    {
        private TestComponent testComponent;

        [SetUp]
        public void Setup()
        {
            LogAssert.ignoreFailingMessages = true;

            Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList().ForEach(Object.DestroyImmediate);

            GameObject root = new("Root");

            GameObject child = new("Child");
            child.AddComponent<TestScope>();
            child.transform.SetParent(root.transform);

            root.AddComponent<InjectableService>();
            testComponent = child.AddComponent<TestComponent>();

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
                    .FromScopeParent();

                BindComponent<IInjectableService, InjectableService>()
                    .FromScopeParent();
            }
        }
    }
}