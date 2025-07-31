using System.Linq;
using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Plugins.Saneject.Runtime.Scopes;
using Tests.Runtime.Legacy;
using UnityEngine;
using UnityEngine.TestTools;
using TestComponent = Tests.Runtime.Legacy.TestComponent;

namespace Tests.Editor.Legacy.Misc
{
    public class InterfaceBindingIsolationTest
    {
        private TestComponent testComponent;

        [SetUp]
        public void Setup()
        {
            LogAssert.ignoreFailingMessages = true;

            Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .ToList()
                .ForEach(Object.DestroyImmediate);

            GameObject root = new("Root");
            GameObject mid = new("Mid");
            mid.transform.SetParent(root.transform);

            GameObject leaf = new("Leaf");
            leaf.transform.SetParent(mid.transform);
            leaf.AddComponent<InjectableService>();

            root.AddComponent<StrictConcreteScope>();
            testComponent = mid.AddComponent<TestComponent>();

            DependencyInjector.InjectSceneDependencies();
        }

        [Test]
        public void InterfaceBindingDoesNotInjectConcreteField()
        {
            Assert.IsNull(testComponent.Service); // concrete field should not be populated
            Assert.NotNull(testComponent.ServiceInterface); // interface field should be populated
        }

        public class StrictConcreteScope : Scope
        {
            public override void ConfigureBindings()
            {
                BindComponent<IInjectableService, InjectableService>()
                    .FromDescendants();
            }
        }
    }
}