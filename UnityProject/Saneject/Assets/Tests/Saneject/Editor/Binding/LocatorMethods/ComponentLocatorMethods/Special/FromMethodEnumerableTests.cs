using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Plugins.Saneject.Editor.Data.Context;
using Tests.Saneject.Fixtures.Scripts;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using Tests.Saneject.Fixtures.Scripts.InjectionTargets;

namespace Tests.Saneject.Editor.Binding.LocatorMethods.ComponentLocatorMethods.Special
{
    public class FromMethodEnumerableTests
    {
        [Test]
        public void FromMethodEnumerable_TConcrete_InjectsToConcreteField()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 2, width: 3, depth: 3);
            scene.Add<ComponentDependency>("Root 2/Child 3/Child 3");
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleConcreteComponentTarget target = scene.Add<SingleConcreteComponentTarget>("Root 1/Child 1");

            // Find dependency
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 2/Child 3/Child 3");

            // Bind
            scope.BindComponent<ComponentDependency>().FromMethod(() => new[] { scene.Get<ComponentDependency>("Root 2/Child 3/Child 3") });

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromMethodEnumerable_TInterface_InjectsToInterfaceField()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 2, width: 3, depth: 3);
            scene.Add<ComponentDependency>("Root 2/Child 3/Child 3");
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleInterfaceTarget target = scene.Add<SingleInterfaceTarget>("Root 1/Child 1");

            // Find dependency
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 2/Child 3/Child 3");

            // Bind
            scope.BindComponent<IDependency>().FromMethod(() => new[] { scene.Get<ComponentDependency>("Root 2/Child 3/Child 3") });

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromMethodEnumerable_TInterfaceTConcrete_InjectsToInterfaceField()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 2, width: 3, depth: 3);
            scene.Add<ComponentDependency>("Root 2/Child 3/Child 3");
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleInterfaceTarget target = scene.Add<SingleInterfaceTarget>("Root 1/Child 1");

            // Find dependency
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 2/Child 3/Child 3");

            // Bind
            scope.BindComponent<IDependency, ComponentDependency>().FromMethod(() => new[] { scene.Get<ComponentDependency>("Root 2/Child 3/Child 3") });

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromMethodEnumerable_TConcrete_InjectsToConcreteCollection()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 2, width: 3, depth: 3);
            TestScope scope = scene.Add<TestScope>("Root 1");
            MultiConcreteComponentTarget target = scene.Add<MultiConcreteComponentTarget>("Root 1/Child 1");

            // Find dependencies
            ComponentDependency[] dependencies =
            {
                scene.Add<ComponentDependency>("Root 2/Child 1/Child 1"),
                scene.Add<ComponentDependency>("Root 2/Child 3/Child 3")
            };

            // Bind
            scope.BindComponents<ComponentDependency>().FromMethod(() => dependencies);

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            CollectionAssert.AreEquivalent(dependencies, target.array);
            CollectionAssert.AreEquivalent(dependencies, target.list);
        }
    }
}