using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Plugins.Saneject.Editor.Data.Context;
using Tests.Saneject.Fixtures.Scripts;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using Tests.Saneject.Fixtures.Scripts.InjectionTargets;

namespace Tests.Saneject.Editor.Injection
{
    public class FieldInjectionTests
    {
        [Test]
        public void Inject_TConcrete_InjectsToConcreteField()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 2);
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleConcreteComponentTarget target = scene.Add<SingleConcreteComponentTarget>("Root 1");

            // Find dependency
            ComponentDependency dependency = scene.Add<ComponentDependency>("Root 1");

            // Bind
            scope.BindComponent<ComponentDependency>().FromSelf();

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void Inject_TConcrete_InjectsToConcreteArrayField()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 2);
            TestScope scope = scene.Add<TestScope>("Root 1");
            MultiConcreteComponentTarget target = scene.Add<MultiConcreteComponentTarget>("Root 1");

            // Find dependencies
            ComponentDependency[] dependencies =
            {
                scene.Add<ComponentDependency>("Root 1"),
                scene.Add<ComponentDependency>("Root 1")
            };

            // Bind
            scope.BindComponents<ComponentDependency>().FromSelf();

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            CollectionAssert.AreEquivalent(dependencies, target.array);
        }

        [Test]
        public void Inject_TConcrete_InjectsToConcreteListField()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 2);
            TestScope scope = scene.Add<TestScope>("Root 1");
            MultiConcreteComponentTarget target = scene.Add<MultiConcreteComponentTarget>("Root 1");

            // Find dependencies
            ComponentDependency[] dependencies =
            {
                scene.Add<ComponentDependency>("Root 1"),
                scene.Add<ComponentDependency>("Root 1")
            };

            // Bind
            scope.BindComponents<ComponentDependency>().FromSelf();

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            CollectionAssert.AreEquivalent(dependencies, target.list);
        }

        [Test]
        public void Inject_TInterface_InjectsToInterfaceField()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 2);
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleInterfaceTarget target = scene.Add<SingleInterfaceTarget>("Root 1");

            // Find dependency
            ComponentDependency dependency = scene.Add<ComponentDependency>("Root 1");

            // Bind
            scope.BindComponent<IDependency>().FromSelf();

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void Inject_TInterface_InjectsToInterfaceArrayField()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 2);
            TestScope scope = scene.Add<TestScope>("Root 1");
            MultiInterfaceTarget target = scene.Add<MultiInterfaceTarget>("Root 1");

            // Find dependencies
            ComponentDependency[] dependencies =
            {
                scene.Add<ComponentDependency>("Root 1"),
                scene.Add<ComponentDependency>("Root 1")
            };

            // Bind
            scope.BindComponents<IDependency>().FromSelf();

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            CollectionAssert.AreEquivalent(dependencies, target.array);
        }

        [Test]
        public void Inject_TInterface_InjectsToInterfaceListField()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 2);
            TestScope scope = scene.Add<TestScope>("Root 1");
            MultiInterfaceTarget target = scene.Add<MultiInterfaceTarget>("Root 1");

            // Find dependencies
            ComponentDependency[] dependencies =
            {
                scene.Add<ComponentDependency>("Root 1"),
                scene.Add<ComponentDependency>("Root 1")
            };

            // Bind
            scope.BindComponents<IDependency>().FromSelf();

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            CollectionAssert.AreEquivalent(dependencies, target.list);
        }
    }
}
