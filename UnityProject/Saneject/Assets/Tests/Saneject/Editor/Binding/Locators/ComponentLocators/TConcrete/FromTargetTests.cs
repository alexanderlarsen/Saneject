using NUnit.Framework;
using Plugins.Saneject.Editor.Data.Context;
using Plugins.Saneject.Editor.Pipeline;
using Tests.Saneject.Fixtures.Scripts;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using Tests.Saneject.Fixtures.Scripts.InjectionTargets;

namespace Tests.Saneject.Editor.Binding.Locators.ComponentLocators.TConcrete
{
    public class FromTargetTests
    {
        [Test]
        public void FromTarget_InjectsFromSelf()
        {
            TestScene scene = TestScene.Create(roots: 1, width: 3, depth: 2);
            scene.AddToAllTransforms<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleConcreteComponentTarget target = scene.Add<SingleConcreteComponentTarget>("Root 1/Child 2");
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 1/Child 2");

            scope.BindComponent<ComponentDependency>().FromTargetSelf();

            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromTargetParent_InjectsFromParent()
        {
            TestScene scene = TestScene.Create(roots: 1, width: 3, depth: 3);
            scene.AddToAllTransforms<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleConcreteComponentTarget target = scene.Add<SingleConcreteComponentTarget>("Root 1/Child 2/Child 1");
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 1/Child 2");

            scope.BindComponent<ComponentDependency>().FromTargetParent();

            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromTargetAncestors_WHEN_IncludeSelfIsFalse_THEN_InjectsFromNearestAncestor()
        {
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 3);
            scene.AddToRoots<ComponentDependency>();
            scene.AddToLeafs<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleConcreteComponentTarget target = scene.Add<SingleConcreteComponentTarget>("Root 1/Child 1/Child 1");
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 1");

            scope.BindComponent<ComponentDependency>().FromTargetAncestors(includeSelf: false);

            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromTargetAncestors_WHEN_IncludeSelfIsTrue_THEN_InjectsFromSelf()
        {
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 3);
            scene.AddToRoots<ComponentDependency>();
            scene.AddToLeafs<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleConcreteComponentTarget target = scene.Add<SingleConcreteComponentTarget>("Root 1/Child 1/Child 1");
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 1/Child 1/Child 1");

            scope.BindComponent<ComponentDependency>().FromTargetAncestors(includeSelf: true);

            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromTargetFirstChild_InjectsFromFirstChild()
        {
            TestScene scene = TestScene.Create(roots: 1, width: 3, depth: 3);
            scene.AddToAllTransforms<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleConcreteComponentTarget target = scene.Add<SingleConcreteComponentTarget>("Root 1");
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 1/Child 1");

            scope.BindComponent<ComponentDependency>().FromTargetFirstChild();

            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromTargetLastChild_InjectsFromLastChild()
        {
            TestScene scene = TestScene.Create(roots: 1, width: 3, depth: 3);
            scene.AddToAllTransforms<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleConcreteComponentTarget target = scene.Add<SingleConcreteComponentTarget>("Root 1");
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 1/Child 3");

            scope.BindComponent<ComponentDependency>().FromTargetLastChild();

            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromTargetChildWithIndex_InjectsFromChildWithIndex()
        {
            TestScene scene = TestScene.Create(roots: 1, width: 3, depth: 3);
            scene.AddToAllTransforms<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleConcreteComponentTarget target = scene.Add<SingleConcreteComponentTarget>("Root 1");
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 1/Child 2");

            scope.BindComponent<ComponentDependency>().FromTargetChildWithIndex(1);

            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromTargetDescendants_WHEN_IncludeSelfIsFalse_THEN_InjectsFromNearestDescendant()
        {
            TestScene scene = TestScene.Create(roots: 1, width: 3, depth: 3);
            scene.AddToRoots<ComponentDependency>();
            scene.AddToLeafs<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleConcreteComponentTarget target = scene.Add<SingleConcreteComponentTarget>("Root 1");
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 1/Child 1/Child 1");

            scope.BindComponent<ComponentDependency>().FromTargetDescendants(includeSelf: false);

            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromTargetDescendants_WHEN_IncludeSelfIsTrue_THEN_InjectsFromSelf()
        {
            TestScene scene = TestScene.Create(roots: 1, width: 3, depth: 3);
            scene.AddToRoots<ComponentDependency>();
            scene.AddToLeafs<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleConcreteComponentTarget target = scene.Add<SingleConcreteComponentTarget>("Root 1");
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 1");

            scope.BindComponent<ComponentDependency>().FromTargetDescendants(includeSelf: true);

            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromTargetSiblings_InjectsFromFirstSibling()
        {
            TestScene scene = TestScene.Create(roots: 1, width: 3, depth: 3);
            scene.AddToAllTransforms<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleConcreteComponentTarget target = scene.Add<SingleConcreteComponentTarget>("Root 1/Child 2/Child 3");
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 1/Child 2/Child 1");

            scope.BindComponent<ComponentDependency>().FromTargetSiblings();

            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }
    }
}