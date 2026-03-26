using NUnit.Framework;
using Plugins.Saneject.Editor.Data.Context;
using Plugins.Saneject.Editor.Pipeline;
using Tests.Saneject.Fixtures.Scripts;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using Tests.Saneject.Fixtures.Scripts.InjectionTargets;

namespace Tests.Saneject.Editor.Binding.LocatorMethods.ComponentLocatorMethods.FromTarget
{
    public class FromTargetParentTests
    {
        [Test]
        public void FromTargetParent_TConcrete_InjectsToConcreteField()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 3, depth: 3);
            scene.AddToAllTransforms<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleConcreteComponentTarget target = scene.Add<SingleConcreteComponentTarget>("Root 1/Child 2/Child 1");

            // Find dependency
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 1/Child 2");

            // Bind
            scope.BindComponent<ComponentDependency>().FromTargetParent();

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromTargetParent_TInterface_InjectsToInterfaceField()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 3, depth: 3);
            scene.AddToAllTransforms<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleInterfaceTarget target = scene.Add<SingleInterfaceTarget>("Root 1/Child 2/Child 1");

            // Find dependency
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 1/Child 2");

            // Bind
            scope.BindComponent<IDependency>().FromTargetParent();

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromTargetParent_TInterfaceTConcrete_InjectsToInterfaceField()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 3, depth: 3);
            scene.AddToAllTransforms<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleInterfaceTarget target = scene.Add<SingleInterfaceTarget>("Root 1/Child 2/Child 1");

            // Find dependency
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 1/Child 2");

            // Bind
            scope.BindComponent<IDependency, ComponentDependency>().FromTargetParent();

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromTargetParent_TConcrete_InjectsToConcreteCollection()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 3, depth: 3);
            TestScope scope = scene.Add<TestScope>("Root 1");
            MultiConcreteComponentTarget target = scene.Add<MultiConcreteComponentTarget>("Root 1/Child 2/Child 1");

            // Find dependencies
            ComponentDependency[] dependencies =
            {
                scene.Add<ComponentDependency>("Root 1/Child 2"),
                scene.Add<ComponentDependency>("Root 1/Child 2")
            };

            // Bind
            scope.BindComponents<ComponentDependency>().FromTargetParent();

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            CollectionAssert.AreEquivalent(dependencies, target.array);
            CollectionAssert.AreEquivalent(dependencies, target.list);
        }
    }
}