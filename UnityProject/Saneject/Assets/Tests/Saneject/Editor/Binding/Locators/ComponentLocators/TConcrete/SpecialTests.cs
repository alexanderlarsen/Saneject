using NUnit.Framework;
using Plugins.Saneject.Editor.Data.Context;
using Plugins.Saneject.Editor.Pipeline;
using Tests.Saneject.Fixtures.Scripts;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using Tests.Saneject.Fixtures.Scripts.InjectionTargets;

namespace Tests.Saneject.Editor.Binding.Locators.ComponentLocators.TConcrete
{
    public class SpecialTests
    {
        [Test]
        public void FromAnywhere_TConcrete_InjectsToConcreteField()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 2, width: 3, depth: 3);
            scene.Add<ComponentDependency>("Root 2/Child 3/Child 3");
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleConcreteComponentTarget target = scene.Add<SingleConcreteComponentTarget>("Root 1/Child 1");
          
            // Find dependency
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 2/Child 3/Child 3");

            // Bind
            scope.BindComponent<ComponentDependency>().FromAnywhere();

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromInstance_TConcrete_InjectsToConcreteField()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 2, width: 3, depth: 3);
            scene.Add<ComponentDependency>("Root 2/Child 3/Child 3");
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleConcreteComponentTarget target = scene.Add<SingleConcreteComponentTarget>("Root 1/Child 1");
         
            // Find dependency
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 2/Child 3/Child 3");

            // Bind
            scope.BindComponent<ComponentDependency>().FromInstance(dependency);

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromMethod_TConcrete_InjectsToConcreteField()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 2, width: 3, depth: 3);
            scene.Add<ComponentDependency>("Root 2/Child 3/Child 3");
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleConcreteComponentTarget target = scene.Add<SingleConcreteComponentTarget>("Root 1/Child 1");
    
            // Find dependency
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 2/Child 3/Child 3");

            // Bind
            scope.BindComponent<ComponentDependency>().FromMethod(() => scene.Get<ComponentDependency>("Root 2/Child 3/Child 3"));

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromMethodEnumerable_TConcrete_InjectsToConcreteField()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 2, width: 3, depth: 3);
            scene.Add<ComponentDependency>("Root 2/Child 3/Child 3");
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleConcreteComponentTarget target = scene.Add<SingleConcreteComponentTarget>("Root 1/Child 1");
           
            // Find dependency
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 2/Child 3/Child 3");

            // Bind
            scope.BindComponent<ComponentDependency>().FromMethod(() => new[]
            {
                scene.Get<ComponentDependency>("Root 2/Child 3/Child 3")
            });

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }
    }
}
