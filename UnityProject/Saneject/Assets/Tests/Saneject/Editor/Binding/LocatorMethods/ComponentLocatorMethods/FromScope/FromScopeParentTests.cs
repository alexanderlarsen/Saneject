using NUnit.Framework;
using Plugins.Saneject.Editor.Data.Context;
using Plugins.Saneject.Editor.Pipeline;
using Tests.Saneject.Fixtures.Scripts;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using Tests.Saneject.Fixtures.Scripts.InjectionTargets;

namespace Tests.Saneject.Editor.Binding.LocatorMethods.ComponentLocatorMethods.FromScope
{
    public class FromScopeParentTests
    {
        [Test]
        public void FromScopeParent_TConcrete_InjectsToConcreteField()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 2);
            scene.AddToAllTransforms<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1/Child 1");
            SingleConcreteComponentTarget target = scene.Add<SingleConcreteComponentTarget>("Root 1/Child 1");

            // Find dependency
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 1");

            // Bind
            scope.BindComponent<ComponentDependency>().FromParent();

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromScopeParent_TInterface_InjectsToInterfaceField()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 2);
            scene.AddToAllTransforms<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1/Child 1");
            SingleInterfaceTarget target = scene.Add<SingleInterfaceTarget>("Root 1/Child 1");

            // Find dependency
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 1");

            // Bind
            scope.BindComponent<IDependency>().FromParent();

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromScopeParent_TInterfaceTConcrete_InjectsToInterfaceField()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 2);
            scene.AddToAllTransforms<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1/Child 1");
            SingleInterfaceTarget target = scene.Add<SingleInterfaceTarget>("Root 1/Child 1");

            // Find dependency
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 1");

            // Bind
            scope.BindComponent<IDependency, ComponentDependency>().FromParent();

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromScopeParent_TConcrete_InjectsToConcreteCollection()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 2);
            TestScope scope = scene.Add<TestScope>("Root 1/Child 1");
            MultiConcreteComponentTarget target = scene.Add<MultiConcreteComponentTarget>("Root 1/Child 1");

            // Find dependencies
            ComponentDependency[] dependencies =
            {
                scene.Add<ComponentDependency>("Root 1"),
                scene.Add<ComponentDependency>("Root 1")
            };

            // Bind
            scope.BindComponents<ComponentDependency>().FromParent();

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            CollectionAssert.AreEquivalent(dependencies, target.array);
            CollectionAssert.AreEquivalent(dependencies, target.list);
        }
    }
}