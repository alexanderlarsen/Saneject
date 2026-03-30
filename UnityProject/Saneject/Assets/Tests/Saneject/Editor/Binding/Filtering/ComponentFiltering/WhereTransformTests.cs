using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Plugins.Saneject.Editor.Data.Context;
using Tests.Saneject.Fixtures.Scripts;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using Tests.Saneject.Fixtures.Scripts.InjectionTargets;

namespace Tests.Saneject.Editor.Binding.Filtering.ComponentFiltering
{
    public class WhereTransformTests
    {
        [Test]
        public void WhereTransform_TInterface_InjectsFilteredComponentToInterfaceField()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 2, depth: 3);
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleInterfaceTarget target = scene.Add<SingleInterfaceTarget>("Root 1");
            scene.Add<ComponentDependency>("Root 1/Child 1");
            ComponentDependency dependency = scene.Add<ComponentDependency>("Root 1/Child 2/Child 2");

            // Bind
            scope.BindComponent<IDependency>()
                .FromAnywhere()
                .WhereTransform(transform =>
                    transform.parent &&
                    transform.parent.name == "Child 2" &&
                    transform.name == "Child 2");

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void WhereTransform_TConcrete_InjectsFilteredComponentsToConcreteCollection()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 2, depth: 3);
            TestScope scope = scene.Add<TestScope>("Root 1");
            MultiConcreteComponentTarget target = scene.Add<MultiConcreteComponentTarget>("Root 1");
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
            scope.BindComponents<ComponentDependency>()
                .FromAnywhere()
                .WhereTransform(transform =>
                    transform.parent &&
                    transform.parent.name == "Root 1");

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