using NUnit.Framework;
using Plugins.Saneject.Editor.Data.Context;
using Plugins.Saneject.Editor.Pipeline;
using Tests.Saneject.Fixtures.Scripts;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using Tests.Saneject.Fixtures.Scripts.InjectionTargets;

namespace Tests.Saneject.Editor.Binding.Components.Filters
{
    public class WhereGameObjectTests
    {
        [Test]
        public void WhereGameObject_TInterface_InjectsFilteredComponentToInterfaceField()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 2, depth: 3);
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleInterfaceTarget target = scene.Add<SingleInterfaceTarget>("Root 1");
            scene.Add<ComponentDependency>("Root 1/Child 2");
            ComponentDependency dependency = scene.Add<ComponentDependency>("Root 1/Child 1/Child 2");

            // Bind
            scope.BindComponent<IDependency>()
                .FromAnywhere()
                .WhereGameObject(gameObject =>
                    gameObject.transform.parent &&
                    gameObject.transform.parent.name == "Child 1" &&
                    gameObject.name == "Child 2");

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }
    }
}