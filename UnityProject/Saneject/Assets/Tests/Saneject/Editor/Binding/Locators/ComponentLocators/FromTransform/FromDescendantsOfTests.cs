using NUnit.Framework;
using Plugins.Saneject.Editor.Data.Context;
using Plugins.Saneject.Editor.Pipeline;
using Tests.Saneject.Fixtures.Scripts;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using Tests.Saneject.Fixtures.Scripts.InjectionTargets;
using UnityEngine;

namespace Tests.Saneject.Editor.Binding.Locators.ComponentLocators.FromTransform
{
    public class FromDescendantsOfTests
    {
        [Test]
        public void FromDescendantsOf_WHEN_IncludeSelfIsFalse_TConcrete_InjectsToConcreteField()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 2, width: 3, depth: 3);
            scene.AddToRoots<ComponentDependency>();
            scene.AddToLeafs<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleConcreteComponentTarget target = scene.Add<SingleConcreteComponentTarget>("Root 1");

            // Find transform and dependency
            Transform transform = scene.GetTransform("Root 2");
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 2/Child 1/Child 1");

            // Bind
            scope.BindComponent<ComponentDependency>().FromDescendantsOf(transform, includeSelf: false);

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromDescendantsOf_WHEN_IncludeSelfIsFalse_TInterface_InjectsToInterfaceField()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 2, width: 3, depth: 3);
            scene.AddToRoots<ComponentDependency>();
            scene.AddToLeafs<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleInterfaceTarget target = scene.Add<SingleInterfaceTarget>("Root 1");

            // Find transform and dependency
            Transform transform = scene.GetTransform("Root 2");
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 2/Child 1/Child 1");

            // Bind
            scope.BindComponent<IDependency>().FromDescendantsOf(transform, includeSelf: false);

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromDescendantsOf_WHEN_IncludeSelfIsFalse_TInterfaceTConcrete_InjectsToInterfaceField()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 2, width: 3, depth: 3);
            scene.AddToRoots<ComponentDependency>();
            scene.AddToLeafs<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleInterfaceTarget target = scene.Add<SingleInterfaceTarget>("Root 1");

            // Find transform and dependency
            Transform transform = scene.GetTransform("Root 2");
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 2/Child 1/Child 1");

            // Bind
            scope.BindComponent<IDependency, ComponentDependency>().FromDescendantsOf(transform, includeSelf: false);

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromDescendantsOf_WHEN_IncludeSelfIsTrue_TConcrete_InjectsToConcreteField()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 2, width: 3, depth: 3);
            scene.AddToRoots<ComponentDependency>();
            scene.AddToLeafs<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleConcreteComponentTarget target = scene.Add<SingleConcreteComponentTarget>("Root 1");

            // Find transform and dependency
            Transform transform = scene.GetTransform("Root 2");
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 2");

            // Bind
            scope.BindComponent<ComponentDependency>().FromDescendantsOf(transform, includeSelf: true);

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromDescendantsOf_WHEN_IncludeSelfIsTrue_TInterface_InjectsToInterfaceField()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 2, width: 3, depth: 3);
            scene.AddToRoots<ComponentDependency>();
            scene.AddToLeafs<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleInterfaceTarget target = scene.Add<SingleInterfaceTarget>("Root 1");

            // Find transform and dependency
            Transform transform = scene.GetTransform("Root 2");
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 2");

            // Bind
            scope.BindComponent<IDependency>().FromDescendantsOf(transform, includeSelf: true);

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromDescendantsOf_WHEN_IncludeSelfIsTrue_TInterfaceTConcrete_InjectsToInterfaceField()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 2, width: 3, depth: 3);
            scene.AddToRoots<ComponentDependency>();
            scene.AddToLeafs<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleInterfaceTarget target = scene.Add<SingleInterfaceTarget>("Root 1");

            // Find transform and dependency
            Transform transform = scene.GetTransform("Root 2");
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 2");

            // Bind
            scope.BindComponent<IDependency, ComponentDependency>().FromDescendantsOf(transform, includeSelf: true);

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }
    }
}