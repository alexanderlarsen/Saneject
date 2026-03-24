using NUnit.Framework;
using Plugins.Saneject.Editor.Data.Context;
using Plugins.Saneject.Editor.Pipeline;
using Tests.Saneject.Fixtures.Scripts;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using Tests.Saneject.Fixtures.Scripts.InjectionTargets;

namespace Tests.Saneject.Editor.Binding.Locators.ComponentLocators.TInterface
{
    public class FromScopeTests
    {
        [Test]
        public void FromScopeSelf_TInterface_InjectsToInterfaceField()
        {
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 2);
            scene.AddToAllTransforms<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleInterfaceTarget target = scene.Add<SingleInterfaceTarget>("Root 1");
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 1");
            scope.BindComponent<IDependency>().FromSelf();
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromScopeParent_TInterface_InjectsToInterfaceField()
        {
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 2);
            scene.AddToAllTransforms<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1/Child 1");
            SingleInterfaceTarget target = scene.Add<SingleInterfaceTarget>("Root 1/Child 1");
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 1");
            scope.BindComponent<IDependency>().FromParent();
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromScopeAncestors_WHEN_IncludeSelfIsFalse_TInterface_InjectsToInterfaceField()
        {
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 3);
            scene.AddToRoots<ComponentDependency>();
            scene.AddToLeafs<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1/Child 1/Child 1");
            SingleInterfaceTarget target = scene.Add<SingleInterfaceTarget>("Root 1/Child 1/Child 1");
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 1");
            scope.BindComponent<IDependency>().FromAncestors(includeSelf: false);
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromScopeAncestors_WHEN_IncludeSelfIsTrue_TInterface_InjectsToInterfaceField()
        {
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 3);
            scene.AddToRoots<ComponentDependency>();
            scene.AddToLeafs<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1/Child 1/Child 1");
            SingleInterfaceTarget target = scene.Add<SingleInterfaceTarget>("Root 1/Child 1/Child 1");
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 1/Child 1/Child 1");
            scope.BindComponent<IDependency>().FromAncestors(includeSelf: true);
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromScopeFirstChild_TInterface_InjectsToInterfaceField()
        {
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 3);
            scene.AddToAllTransforms<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleInterfaceTarget target = scene.Add<SingleInterfaceTarget>("Root 1");
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 1/Child 1");
            scope.BindComponent<IDependency>().FromFirstChild();
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromScopeLastChild_TInterface_InjectsToInterfaceField()
        {
            TestScene scene = TestScene.Create(roots: 1, width: 3, depth: 3);
            scene.AddToAllTransforms<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleInterfaceTarget target = scene.Add<SingleInterfaceTarget>("Root 1");
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 1/Child 3");
            scope.BindComponent<IDependency>().FromLastChild();
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromScopeChildWithIndex_TInterface_InjectsToInterfaceField()
        {
            TestScene scene = TestScene.Create(roots: 1, width: 3, depth: 3);
            scene.AddToAllTransforms<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleInterfaceTarget target = scene.Add<SingleInterfaceTarget>("Root 1");
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 1/Child 2");
            scope.BindComponent<IDependency>().FromChildWithIndex(1);
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromScopeDescendants_WHEN_IncludeSelfIsFalse_TInterface_InjectsToInterfaceField()
        {
            TestScene scene = TestScene.Create(roots: 1, width: 3, depth: 3);
            scene.AddToLeafs<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleInterfaceTarget target = scene.Add<SingleInterfaceTarget>("Root 1/Child 1");
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 1/Child 1/Child 1");
            scope.BindComponent<IDependency>().FromDescendants(includeSelf: false);
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromScopeDescendants_WHEN_IncludeSelfIsTrue_TInterface_InjectsToInterfaceField()
        {
            TestScene scene = TestScene.Create(roots: 1, width: 3, depth: 3);
            scene.AddToRoots<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleInterfaceTarget target = scene.Add<SingleInterfaceTarget>("Root 1");
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 1");
            scope.BindComponent<IDependency>().FromDescendants(includeSelf: true);
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromScopeSiblings_TInterface_InjectsToInterfaceField()
        {
            TestScene scene = TestScene.Create(roots: 1, width: 3, depth: 3);
            scene.AddToAllTransforms<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1/Child 2");
            SingleInterfaceTarget target = scene.Add<SingleInterfaceTarget>("Root 1/Child 2");
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 1/Child 1");
            scope.BindComponent<IDependency>().FromSiblings();
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }
    }
}