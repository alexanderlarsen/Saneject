using System.Linq;
using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Plugins.Saneject.Runtime.Scopes;
using Tests.Runtime;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Editor.BindingResolution.ScopeRelative
{
    public class FromScopeSiblingsTest
    {
        private TestComponent testComponent;

        [SetUp]
        public void Setup()
        {
            LogAssert.ignoreFailingMessages = true;

            Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .ToList().ForEach(Object.DestroyImmediate);

            GameObject parent = new("Parent");

            GameObject siblingA = new("SiblingA");
            siblingA.transform.SetParent(parent.transform);

            GameObject siblingB = new("SiblingB");
            siblingB.transform.SetParent(parent.transform);

            siblingA.AddComponent<InjectableService>();
            testComponent = siblingB.AddComponent<TestComponent>();
            siblingB.AddComponent<TestScope>();

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
                    .FromScopeSiblings();

                BindComponent<IInjectableService, InjectableService>()
                    .FromScopeSiblings();
            }
        }
    }
}