using NUnit.Framework;
using Plugins.Saneject.Editor.Data.Context;
using Plugins.Saneject.Editor.Pipeline;
using Tests.Saneject.Fixtures.Scripts;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using Tests.Saneject.Fixtures.Scripts.InjectionTargets;
using UnityEngine;

namespace Tests.Saneject.Editor.Binding.Locators.ComponentLocators.FromTransform
{
    public class FromSiblingsOfTConcreteTests
    {
        [Test]
        public void FromSiblingsOf_TConcrete_InjectsToConcreteField()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 2, width: 3, depth: 3);
            scene.AddToAllTransforms<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleConcreteComponentTarget target = scene.Add<SingleConcreteComponentTarget>("Root 1");

            // Find transform and dependency
            Transform transform = scene.GetTransform("Root 2/Child 2/Child 3");
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 2/Child 2/Child 1");

            // Bind
            scope.BindComponent<ComponentDependency>().FromSiblingsOf(transform);

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }
    }

    public class FromSiblingsOfTInterfaceTests
    {
        [Test]
        public void FromSiblingsOf_TInterface_InjectsToInterfaceField()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 2, width: 3, depth: 3);
            scene.AddToAllTransforms<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleInterfaceTarget target = scene.Add<SingleInterfaceTarget>("Root 1");

            // Find transform and dependency
            Transform transform = scene.GetTransform("Root 2/Child 2/Child 3");
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 2/Child 2/Child 1");

            // Bind
            scope.BindComponent<IDependency>().FromSiblingsOf(transform);

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }
    }

    public class FromSiblingsOfTInterfaceTConcreteTests
    {
        [Test]
        public void FromSiblingsOf_TInterface_InjectsToInterfaceField()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 2, width: 3, depth: 3);
            scene.AddToAllTransforms<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleInterfaceTarget target = scene.Add<SingleInterfaceTarget>("Root 1");

            // Find transform and dependency
            Transform transform = scene.GetTransform("Root 2/Child 2/Child 3");
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 2/Child 2/Child 1");

            // Bind
            scope.BindComponent<IDependency, ComponentDependency>().FromSiblingsOf(transform);

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }
    }
}