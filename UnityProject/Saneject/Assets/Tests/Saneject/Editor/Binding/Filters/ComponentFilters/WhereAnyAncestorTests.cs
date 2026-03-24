using NUnit.Framework;
using Plugins.Saneject.Editor.Data.Context;
using Plugins.Saneject.Editor.Pipeline;
using Tests.Saneject.Fixtures.Scripts;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using Tests.Saneject.Fixtures.Scripts.InjectionTargets;

namespace Tests.Saneject.Editor.Binding.Filters.ComponentFilters
{
    public class WhereAnyAncestorTests
    {
        [Test]
        public void WhereAnyAncestor_WHEN_IncludeSelfIsFalse_TConcrete_InjectsFilteredComponentToConcreteField()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 2, depth: 4);
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleConcreteComponentTarget target = scene.Add<SingleConcreteComponentTarget>("Root 1");
            scene.Add<ComponentDependency>("Root 1/Child 1/Child 1/Child 1");
            ComponentDependency dependency = scene.Add<ComponentDependency>("Root 1/Child 2/Child 1/Child 1");

            // Bind
            scope.BindComponent<ComponentDependency>()
                .FromAnywhere()
                .WhereAnyAncestor(transform => transform.name == "Child 2");

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void WhereAnyAncestor_WHEN_IncludeSelfIsTrue_TConcrete_InjectsFilteredComponentToConcreteField()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 2, depth: 3);
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleConcreteComponentTarget target = scene.Add<SingleConcreteComponentTarget>("Root 1");
            scene.Add<ComponentDependency>("Root 1/Child 1/Child 2");
            ComponentDependency dependency = scene.Add<ComponentDependency>("Root 1/Child 2");

            // Bind
            scope.BindComponent<ComponentDependency>()
                .FromAnywhere()
                .WhereAnyAncestor(
                    transform =>
                        transform.name == "Child 2" &&
                        transform.parent &&
                        transform.parent.name == "Root 1",
                    includeSelf: true);

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void WhereAnyAncestor_WHEN_MaxDepthIs1_TConcrete_InjectsOnlyClosestMatchingAncestorsToConcreteCollection()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 2, depth: 4);
            TestScope scope = scene.Add<TestScope>("Root 1");
            MultiConcreteComponentTarget target = scene.Add<MultiConcreteComponentTarget>("Root 1");
            ComponentDependency dependency = scene.Add<ComponentDependency>("Root 1/Child 1/Child 1");
            scene.Add<ComponentDependency>("Root 1/Child 1/Child 2/Child 1");
            scene.Add<ComponentDependency>("Root 1/Child 2/Child 1");

            // Find dependency
            ComponentDependency[] dependencies =
            {
                dependency
            };

            // Bind
            scope.BindComponents<ComponentDependency>()
                .FromAnywhere()
                .WhereAnyAncestor(transform => transform.name == "Child 1", maxDepth: 1);

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            CollectionAssert.AreEquivalent(dependencies, target.array);
            CollectionAssert.AreEquivalent(dependencies, target.list);
        }
    }
}
