using NUnit.Framework;
using Plugins.Saneject.Editor.Data.Context;
using Plugins.Saneject.Editor.Pipeline;
using Tests.Saneject.Fixtures.Scripts;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using Tests.Saneject.Fixtures.Scripts.InjectionTargets;
using UnityEngine;

namespace Tests.Saneject.Editor.Binding.Locators.ComponentLocators.TInterfaceTConcrete
{
    public class FromTransformTests
    {
        [Test]
        public void From_TInterface_InjectsToInterfaceField()
        {
            TestScene scene = TestScene.Create(roots: 2, width: 3, depth: 3);
            scene.AddToAllTransforms<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleInterfaceTarget target = scene.Add<SingleInterfaceTarget>("Root 1/Child 1");
            Transform transform = scene.GetTransform("Root 2/Child 3/Child 3");
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 2/Child 3/Child 3");
            scope.BindComponent<IDependency, ComponentDependency>().From(transform);
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromParentOf_TInterface_InjectsToInterfaceField()
        {
            TestScene scene = TestScene.Create(roots: 2, width: 3, depth: 3);
            scene.AddToAllTransforms<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleInterfaceTarget target = scene.Add<SingleInterfaceTarget>("Root 1/Child 1");
            Transform transform = scene.GetTransform("Root 2/Child 3/Child 3");
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 2/Child 3");
            scope.BindComponent<IDependency, ComponentDependency>().FromParentOf(transform);
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromAncestorsOf_WHEN_IncludeSelfIsFalse_TInterface_InjectsToInterfaceField()
        {
            TestScene scene = TestScene.Create(roots: 2, width: 1, depth: 3);
            scene.AddToRoots<ComponentDependency>();
            scene.AddToLeafs<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleInterfaceTarget target = scene.Add<SingleInterfaceTarget>("Root 1/Child 1");
            Transform transform = scene.GetTransform("Root 2/Child 1/Child 1");
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 2");
            scope.BindComponent<IDependency, ComponentDependency>().FromAncestorsOf(transform, includeSelf: false);
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromAncestorsOf_WHEN_IncludeSelfIsTrue_TInterface_InjectsToInterfaceField()
        {
            TestScene scene = TestScene.Create(roots: 2, width: 1, depth: 3);
            scene.AddToRoots<ComponentDependency>();
            scene.AddToLeafs<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleInterfaceTarget target = scene.Add<SingleInterfaceTarget>("Root 1/Child 1");
            Transform transform = scene.GetTransform("Root 2/Child 1/Child 1");
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 2/Child 1/Child 1");
            scope.BindComponent<IDependency, ComponentDependency>().FromAncestorsOf(transform, includeSelf: true);
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromFirstChildOf_TInterface_InjectsToInterfaceField()
        {
            TestScene scene = TestScene.Create(roots: 2, width: 3, depth: 3);
            scene.AddToAllTransforms<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleInterfaceTarget target = scene.Add<SingleInterfaceTarget>("Root 1");
            Transform transform = scene.GetTransform("Root 2");
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 2/Child 1");
            scope.BindComponent<IDependency, ComponentDependency>().FromFirstChildOf(transform);
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromLastChildOf_TInterface_InjectsToInterfaceField()
        {
            TestScene scene = TestScene.Create(roots: 2, width: 3, depth: 3);
            scene.AddToAllTransforms<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleInterfaceTarget target = scene.Add<SingleInterfaceTarget>("Root 1");
            Transform transform = scene.GetTransform("Root 2");
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 2/Child 3");
            scope.BindComponent<IDependency, ComponentDependency>().FromLastChildOf(transform);
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromChildWithIndexOf_TInterface_InjectsToInterfaceField()
        {
            TestScene scene = TestScene.Create(roots: 2, width: 3, depth: 3);
            scene.AddToAllTransforms<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleInterfaceTarget target = scene.Add<SingleInterfaceTarget>("Root 1");
            Transform transform = scene.GetTransform("Root 2");
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 2/Child 2");
            scope.BindComponent<IDependency, ComponentDependency>().FromChildWithIndexOf(transform, 1);
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromDescendantsOf_WHEN_IncludeSelfIsFalse_TInterface_InjectsToInterfaceField()
        {
            TestScene scene = TestScene.Create(roots: 2, width: 3, depth: 3);
            scene.AddToRoots<ComponentDependency>();
            scene.AddToLeafs<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleInterfaceTarget target = scene.Add<SingleInterfaceTarget>("Root 1");
            Transform transform = scene.GetTransform("Root 2");
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 2/Child 1/Child 1");
            scope.BindComponent<IDependency, ComponentDependency>().FromDescendantsOf(transform, includeSelf: false);
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromDescendantsOf_WHEN_IncludeSelfIsTrue_TInterface_InjectsToInterfaceField()
        {
            TestScene scene = TestScene.Create(roots: 2, width: 3, depth: 3);
            scene.AddToRoots<ComponentDependency>();
            scene.AddToLeafs<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleInterfaceTarget target = scene.Add<SingleInterfaceTarget>("Root 1");
            Transform transform = scene.GetTransform("Root 2");
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 2");
            scope.BindComponent<IDependency, ComponentDependency>().FromDescendantsOf(transform, includeSelf: true);
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromSiblingsOf_TInterface_InjectsToInterfaceField()
        {
            TestScene scene = TestScene.Create(roots: 2, width: 3, depth: 3);
            scene.AddToAllTransforms<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleInterfaceTarget target = scene.Add<SingleInterfaceTarget>("Root 1");
            Transform transform = scene.GetTransform("Root 2/Child 2/Child 3");
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 2/Child 2/Child 1");
            scope.BindComponent<IDependency, ComponentDependency>().FromSiblingsOf(transform);
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }
    }
}