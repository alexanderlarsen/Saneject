using NUnit.Framework;
using Plugins.Saneject.Editor.Data.Context;
using Plugins.Saneject.Editor.Pipeline;
using Tests.Saneject.Fixtures.Scripts;

namespace Tests.Saneject.Editor.Binding.Locators.ComponentLocators
{
    public class SpecialTests
    {
        [Test]
        public void FromAnywhere_InjectsFromAnywhereInScene()
        {
            TestScene scene = TestScene.Create(roots: 2, width: 3, depth: 3);
            scene.Add<ComponentDependency>("Root 2/Child 3/Child 3");

            TestScope scope = scene.Add<TestScope>("Root 1");
            ConcreteTarget target = scene.Add<ConcreteTarget>("Root 1/Child 1");
            ComponentDependency expected = scene.Get<ComponentDependency>("Root 2/Child 3/Child 3");

            scope.BindComponent<ComponentDependency>().FromAnywhere();

            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            Assert.AreEqual(expected, target.dependency);
        }

        [Test]
        public void FromInstance_InjectsSpecifiedInstance()
        {
            TestScene scene = TestScene.Create(roots: 2, width: 3, depth: 3);
            scene.Add<ComponentDependency>("Root 2/Child 3/Child 3");

            TestScope scope = scene.Add<TestScope>("Root 1");
            ConcreteTarget target = scene.Add<ConcreteTarget>("Root 1/Child 1");
            ComponentDependency expected = scene.Get<ComponentDependency>("Root 2/Child 3/Child 3");

            scope.BindComponent<ComponentDependency>().FromInstance(expected);

            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            Assert.AreEqual(expected, target.dependency);
        }

        [Test]
        public void FromMethod_InjectsComponentReturnedByMethod()
        {
            TestScene scene = TestScene.Create(roots: 2, width: 3, depth: 3);
            scene.Add<ComponentDependency>("Root 2/Child 3/Child 3");

            TestScope scope = scene.Add<TestScope>("Root 1");
            ConcreteTarget target = scene.Add<ConcreteTarget>("Root 1/Child 1");
            ComponentDependency expected = scene.Get<ComponentDependency>("Root 2/Child 3/Child 3");

            scope.BindComponent<ComponentDependency>().FromMethod(() => scene.Get<ComponentDependency>("Root 2/Child 3/Child 3"));

            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            Assert.AreEqual(expected, target.dependency);
        }

        [Test]
        public void FromMethodEnumerable_InjectsSingleComponentReturnedByMethod()
        {
            TestScene scene = TestScene.Create(roots: 2, width: 3, depth: 3);
            scene.Add<ComponentDependency>("Root 2/Child 3/Child 3");

            TestScope scope = scene.Add<TestScope>("Root 1");
            ConcreteTarget target = scene.Add<ConcreteTarget>("Root 1/Child 1");
            ComponentDependency expected = scene.Get<ComponentDependency>("Root 2/Child 3/Child 3");

            scope.BindComponent<ComponentDependency>().FromMethod(() => new[] { scene.Get<ComponentDependency>("Root 2/Child 3/Child 3") });

            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            Assert.AreEqual(expected, target.dependency);
        }
    }
}
