using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Plugins.Saneject.Editor.Data.Context;
using Tests.Saneject.Fixtures.Scripts;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using Tests.Saneject.Fixtures.Scripts.InjectionTargets;
using UnityEngine;

namespace Tests.Saneject.Editor.Binding.LocatorMethods.ComponentLocatorMethods.FromTransform
{
    public class FromChildWithIndexOfTests
    {
        [Test]
        public void FromChildWithIndexOf_TConcrete_InjectsToConcreteField()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 2, width: 3, depth: 3);
            scene.AddToAllTransforms<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleConcreteComponentTarget target = scene.Add<SingleConcreteComponentTarget>("Root 1");

            // Find transform and dependency
            Transform transform = scene.GetTransform("Root 2");
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 2/Child 2");

            // Bind
            scope.BindComponent<ComponentDependency>().FromChildWithIndexOf(transform, 1);

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromChildWithIndexOf_TInterface_InjectsToInterfaceField()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 2, width: 3, depth: 3);
            scene.AddToAllTransforms<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleInterfaceTarget target = scene.Add<SingleInterfaceTarget>("Root 1");

            // Find transform and dependency
            Transform transform = scene.GetTransform("Root 2");
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 2/Child 2");

            // Bind
            scope.BindComponent<IDependency>().FromChildWithIndexOf(transform, 1);

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromChildWithIndexOf_TInterfaceTConcrete_InjectsToInterfaceField()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 2, width: 3, depth: 3);
            scene.AddToAllTransforms<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleInterfaceTarget target = scene.Add<SingleInterfaceTarget>("Root 1");

            // Find transform and dependency
            Transform transform = scene.GetTransform("Root 2");
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 2/Child 2");

            // Bind
            scope.BindComponent<IDependency, ComponentDependency>().FromChildWithIndexOf(transform, 1);

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromChildWithIndexOf_TConcrete_InjectsToConcreteCollection()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 2, width: 3, depth: 3);
            TestScope scope = scene.Add<TestScope>("Root 1");
            MultiConcreteComponentTarget target = scene.Add<MultiConcreteComponentTarget>("Root 1");

            // Find transform and dependencies
            Transform transform = scene.GetTransform("Root 2");

            ComponentDependency[] dependencies =
            {
                scene.Add<ComponentDependency>("Root 2/Child 2"),
                scene.Add<ComponentDependency>("Root 2/Child 2")
            };

            // Bind
            scope.BindComponents<ComponentDependency>().FromChildWithIndexOf(transform, 1);

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            CollectionAssert.AreEquivalent(dependencies, target.array);
            CollectionAssert.AreEquivalent(dependencies, target.list);
        }
    }
}