using NUnit.Framework;
using Plugins.Saneject.Editor.Data.Context;
using Plugins.Saneject.Editor.Pipeline;
using Tests.Saneject.Fixtures.Scripts;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using Tests.Saneject.Fixtures.Scripts.InjectionTargets;

namespace Tests.Saneject.Editor.Binding.Filtering.ComponentFiltering
{
    public class WhereFirstChildTests
    {
        [Test]
        public void WhereFirstChild_TConcrete_InjectsFilteredComponentToConcreteField()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 2, depth: 3);
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleConcreteComponentTarget target = scene.Add<SingleConcreteComponentTarget>("Root 1");
            scene.Add<ComponentDependency>("Root 1/Child 1");
            ComponentDependency dependency = scene.Add<ComponentDependency>("Root 1/Child 2");

            // Bind
            scope.BindComponent<ComponentDependency>()
                .FromAnywhere()
                .WhereFirstChild(child =>
                    child.parent &&
                    child.parent.name == "Child 2" &&
                    child.name == "Child 1");

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }
    }
}