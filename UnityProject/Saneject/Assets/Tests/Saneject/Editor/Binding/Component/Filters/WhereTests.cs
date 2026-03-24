using NUnit.Framework;
using Plugins.Saneject.Editor.Data.Context;
using Plugins.Saneject.Editor.Pipeline;
using Tests.Saneject.Fixtures.Scripts;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using Tests.Saneject.Fixtures.Scripts.InjectionTargets;

namespace Tests.Saneject.Editor.Binding.Components.Filters
{
    public class WhereTests
    {
        [Test]
        public void Where_TConcrete_InjectsFilteredComponentToConcreteField()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 2, depth: 3);
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleConcreteComponentTarget target = scene.Add<SingleConcreteComponentTarget>("Root 1");
            scene.Add<ComponentDependency>("Root 1/Child 1");
            ComponentDependency dependency = scene.Add<ComponentDependency>("Root 1/Child 2/Child 2");

            // Bind
            scope.BindComponent<ComponentDependency>()
                .FromAnywhere()
                .Where(component =>
                    component.transform.parent &&
                    component.transform.parent.name == "Child 2" &&
                    component.name == "Child 2");

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void Where_TInterface_InjectsFilteredComponentsToInterfaceCollection()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 2, depth: 3);
            TestScope scope = scene.Add<TestScope>("Root 1");
            MultiInterfaceTarget target = scene.Add<MultiInterfaceTarget>("Root 1");
            ComponentDependency dependencyA = scene.Add<ComponentDependency>("Root 1/Child 1");
            ComponentDependency dependencyB = scene.Add<ComponentDependency>("Root 1/Child 2");
            scene.Add<ComponentDependency>("Root 1/Child 1/Child 1");

            // Find dependency
            ComponentDependency[] dependencies =
            {
                dependencyA,
                dependencyB
            };

            // Bind
            scope.BindComponents<IDependency>()
                .FromAnywhere()
                .Where(dependency =>
                    dependency is ComponentDependency component &&
                    component.transform.parent &&
                    component.transform.parent.name == "Root 1");

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            CollectionAssert.IsNotEmpty(dependencies);
            CollectionAssert.AllItemsAreNotNull(dependencies);
            CollectionAssert.AllItemsAreUnique(dependencies);
            CollectionAssert.AllItemsAreInstancesOfType(dependencies, typeof(ComponentDependency));

            CollectionAssert.AreEquivalent(dependencies, target.array);
            CollectionAssert.AreEquivalent(dependencies, target.list);
        }
    }
}