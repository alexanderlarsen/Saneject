using System.Linq;
using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Plugins.Saneject.Runtime.Scopes;
using Tests.Runtime;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Editor.BindingResolution.ScopeRelative
{
    public class FromScopeAncestorsTest
    {
        private TestComponent testComponent;

        [SetUp]
        public void Setup()
        {
            LogAssert.ignoreFailingMessages = true;

            Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList().ForEach(Object.DestroyImmediate);

            GameObject root = new("Root");

            GameObject mid = new("Mid");
            mid.transform.SetParent(root.transform);

            GameObject leaf = new("Leaf");
            leaf.transform.SetParent(mid.transform);

            root.AddComponent<InjectableService>();
            testComponent = leaf.AddComponent<TestComponent>();
            leaf.AddComponent<TestScope>(); // Scope must be on the leaf (or lower than the service)

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
                    .FromScopeAncestors();

                BindComponent<IInjectableService, InjectableService>()
                    .FromScopeAncestors();
            }
        }
    }
}