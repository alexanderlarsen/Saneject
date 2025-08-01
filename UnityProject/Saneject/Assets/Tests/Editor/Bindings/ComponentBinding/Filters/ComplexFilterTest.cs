using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Tests.Runtime;
using Tests.Runtime.Component;
using UnityEngine;
using ComponentRequester = Tests.Runtime.Component.ComponentRequester;

namespace Tests.Editor.Bindings.ComponentBinding.Filters
{
    public class ComplexFilterTest : BaseBindingTest
    {
        private GameObject root, target, ignored1, ignored2;

        [Test]
        public void InjectsOnlyWhenAllFiltersPass()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            ComponentRequester requester = root.AddComponent<ComponentRequester>();
            InjectableComponent passing = target.AddComponent<InjectableComponent>();
            InjectableComponent failing1 = ignored1.AddComponent<InjectableComponent>();
            InjectableComponent failing2 = ignored2.AddComponent<InjectableComponent>();

            // Tag, name, layer setup
            target.tag = "Test";
            target.name = "Filter_OK";
            target.layer = 3;

            ignored1.name = "Fail_A";
            ignored1.layer = 1;

            ignored2.name = "Fail_B";
            ignored2.layer = 2;

            // Set component states
            passing.enabled = true;
            failing1.enabled = true;
            failing2.enabled = true;

            target.SetActive(true);
            ignored1.SetActive(true);
            ignored2.SetActive(false);

            // Set up bindings
            BindComponent<InjectableComponent>(scope)
                .FromRootDescendants()
                .WhereIsEnabled()
                .WhereIsActiveAndEnabled()
                .WhereComponentIndexIs(0)
                .WhereIsLastComponentSibling()
                .WhereNameContains("OK")
                .WhereTagIs("Test")
                .WhereLayerIs(3)
                .WhereActiveInHierarchy()
                .WhereTargetIs<ComponentRequester>()
                .Where(c => c.enabled); // redundant on purpose

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.AreEqual(passing, requester.concreteComponent);
        }

        protected override void CreateHierarchy()
        {
            root = new GameObject();
            target = new GameObject();
            ignored1 = new GameObject();
            ignored2 = new GameObject();

            target.transform.SetParent(root.transform);
            ignored1.transform.SetParent(root.transform);
            ignored2.transform.SetParent(root.transform);
        }
    }
}