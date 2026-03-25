using NUnit.Framework;
using Plugins.Saneject.Editor.Data.Context;
using Plugins.Saneject.Editor.Pipeline;
using Tests.Saneject.Fixtures.Scripts;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using Tests.Saneject.Fixtures.Scripts.InjectionTargets;

namespace Tests.Saneject.Editor.Binding.Component.Filters
{
    public class WhereAnySiblingTests
    {
        [Test]
        public void WhereAnySibling_TConcrete_InjectsFilteredComponentToConcreteField()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 2, depth: 3);
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleConcreteComponentTarget target = scene.Add<SingleConcreteComponentTarget>("Root 1");
            ComponentDependency dependency = scene.Add<ComponentDependency>("Root 1/Child 1");
            scene.Add<ComponentDependency>("Root 1/Child 1/Child 1");

            // Bind
            scope.BindComponent<ComponentDependency>()
                .FromAnywhere()
                .WhereAnySibling(sibling =>
                    sibling.parent &&
                    sibling.parent.name == "Root 1" &&
                    sibling.name == "Child 2");

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }
    }
}