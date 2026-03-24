using NUnit.Framework;
using Plugins.Saneject.Editor.Data.Context;
using Plugins.Saneject.Editor.Pipeline;
using Tests.Saneject.Fixtures.Scripts;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using Tests.Saneject.Fixtures.Scripts.InjectionTargets;

namespace Tests.Saneject.Editor.Binding.Locators.ComponentLocators.TInterfaceTConcrete
{
    public class FromTargetTests
    {
        [Test]
        public void FromTargetSelf_TInterface_InjectsToInterfaceField()
        {
            TestScene scene = TestScene.Create(roots: 1, width: 3, depth: 2);
            scene.AddToAllTransforms<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleInterfaceTarget target = scene.Add<SingleInterfaceTarget>("Root 1/Child 2");
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 1/Child 2");
            scope.BindComponent<IDependency, ComponentDependency>().FromTargetSelf();
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromTargetParent_TInterface_InjectsToInterfaceField()
        {
            TestScene scene = TestScene.Create(roots: 1, width: 3, depth: 3);
            scene.AddToAllTransforms<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleInterfaceTarget target = scene.Add<SingleInterfaceTarget>("Root 1/Child 2/Child 1");
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 1/Child 2");
            scope.BindComponent<IDependency, ComponentDependency>().FromTargetParent();
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromTargetAncestors_WHEN_IncludeSelfIsFalse_TInterface_InjectsToInterfaceField()
        {
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 3);
            scene.AddToRoots<ComponentDependency>();
            scene.AddToLeafs<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleInterfaceTarget target = scene.Add<SingleInterfaceTarget>("Root 1/Child 1/Child 1");
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 1");
            scope.BindComponent<IDependency, ComponentDependency>().FromTargetAncestors(includeSelf: false);
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromTargetAncestors_WHEN_IncludeSelfIsTrue_TInterface_InjectsToInterfaceField()
        {
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 3);
            scene.AddToRoots<ComponentDependency>();
            scene.AddToLeafs<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleInterfaceTarget target = scene.Add<SingleInterfaceTarget>("Root 1/Child 1/Child 1");
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 1/Child 1/Child 1");
            scope.BindComponent<IDependency, ComponentDependency>().FromTargetAncestors(includeSelf: true);
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromTargetFirstChild_TInterface_InjectsToInterfaceField()
        {
            TestScene scene = TestScene.Create(roots: 1, width: 3, depth: 3);
            scene.AddToAllTransforms<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleInterfaceTarget target = scene.Add<SingleInterfaceTarget>("Root 1");
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 1/Child 1");
            scope.BindComponent<IDependency, ComponentDependency>().FromTargetFirstChild();
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromTargetLastChild_TInterface_InjectsToInterfaceField()
        {
            TestScene scene = TestScene.Create(roots: 1, width: 3, depth: 3);
            scene.AddToAllTransforms<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleInterfaceTarget target = scene.Add<SingleInterfaceTarget>("Root 1");
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 1/Child 3");
            scope.BindComponent<IDependency, ComponentDependency>().FromTargetLastChild();
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromTargetChildWithIndex_TInterface_InjectsToInterfaceField()
        {
            TestScene scene = TestScene.Create(roots: 1, width: 3, depth: 3);
            scene.AddToAllTransforms<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleInterfaceTarget target = scene.Add<SingleInterfaceTarget>("Root 1");
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 1/Child 2");
            scope.BindComponent<IDependency, ComponentDependency>().FromTargetChildWithIndex(1);
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromTargetDescendants_WHEN_IncludeSelfIsFalse_TInterface_InjectsToInterfaceField()
        {
            TestScene scene = TestScene.Create(roots: 1, width: 3, depth: 3);
            scene.AddToRoots<ComponentDependency>();
            scene.AddToLeafs<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleInterfaceTarget target = scene.Add<SingleInterfaceTarget>("Root 1");
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 1/Child 1/Child 1");
            scope.BindComponent<IDependency, ComponentDependency>().FromTargetDescendants(includeSelf: false);
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromTargetDescendants_WHEN_IncludeSelfIsTrue_TInterface_InjectsToInterfaceField()
        {
            TestScene scene = TestScene.Create(roots: 1, width: 3, depth: 3);
            scene.AddToRoots<ComponentDependency>();
            scene.AddToLeafs<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleInterfaceTarget target = scene.Add<SingleInterfaceTarget>("Root 1");
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 1");
            scope.BindComponent<IDependency, ComponentDependency>().FromTargetDescendants(includeSelf: true);
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromTargetSiblings_TInterface_InjectsToInterfaceField()
        {
            TestScene scene = TestScene.Create(roots: 1, width: 3, depth: 3);
            scene.AddToAllTransforms<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleInterfaceTarget target = scene.Add<SingleInterfaceTarget>("Root 1/Child 2/Child 3");
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 1/Child 2/Child 1");
            scope.BindComponent<IDependency, ComponentDependency>().FromTargetSiblings();
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }
    }
}