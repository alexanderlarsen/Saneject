using NUnit.Framework;
using Plugins.Saneject.Editor.Data.Context;
using Plugins.Saneject.Editor.Pipeline;
using Tests.Saneject.Fixtures.Scripts;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using Tests.Saneject.Fixtures.Scripts.InjectionTargets;

namespace Tests.Saneject.Editor.Injection.InjectionSites
{
    public class MethodInjectionTests
    {
        [Test]
        public void Inject_TConcrete_InjectsToConcreteParameter()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 2);
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleConcreteComponentMethodTarget target = scene.Add<SingleConcreteComponentMethodTarget>("Root 1");

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
        public void Inject_TConcrete_InjectsToConcreteArrayParameter()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 2);
            TestScope scope = scene.Add<TestScope>("Root 1");
            MultiConcreteComponentMethodTarget target = scene.Add<MultiConcreteComponentMethodTarget>("Root 1");

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
        public void Inject_TConcrete_InjectsToConcreteListParameter()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 2);
            TestScope scope = scene.Add<TestScope>("Root 1");
            MultiConcreteComponentMethodTarget target = scene.Add<MultiConcreteComponentMethodTarget>("Root 1");

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
        public void Inject_TInterface_InjectsToInterfaceParameter()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 2);
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleInterfaceMethodTarget target = scene.Add<SingleInterfaceMethodTarget>("Root 1");

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
        public void Inject_TInterface_InjectsToInterfaceArrayParameter()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 2);
            TestScope scope = scene.Add<TestScope>("Root 1");
            MultiInterfaceMethodTarget target = scene.Add<MultiInterfaceMethodTarget>("Root 1");

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
        public void Inject_TInterface_InjectsToInterfaceListParameter()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 2);
            TestScope scope = scene.Add<TestScope>("Root 1");
            MultiInterfaceMethodTarget target = scene.Add<MultiInterfaceMethodTarget>("Root 1");

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

        [Test]
        public void Inject_MultipleParameters_InjectsToMixedParameters()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 2);
            TestScope scope = scene.Add<TestScope>("Root 1");
            MixedMethodTarget target = scene.Add<MixedMethodTarget>("Root 1");

            // Find dependencies
            ComponentDependency singleDependency = scene.Add<ComponentDependency>("Root 1");
            ComponentDependency[] dependencies =
            {
                singleDependency,
                scene.Add<ComponentDependency>("Root 1/Child 1")
            };

            // Bind
            scope.BindComponent<ComponentDependency>().FromSelf();
            scope.BindComponents<ComponentDependency>().FromDescendants(includeSelf: true);
            scope.BindComponent<IDependency>().FromSelf();
            scope.BindComponents<IDependency>().FromDescendants(includeSelf: true);

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(singleDependency, Is.Not.Null);
            Assert.That(target.singleConcreteDependency, Is.EqualTo(singleDependency));
            Assert.That(target.singleInterfaceDependency, Is.EqualTo(singleDependency));
            CollectionAssert.AreEquivalent(dependencies, target.concreteArray);
            CollectionAssert.AreEquivalent(dependencies, target.interfaceList);
            Assert.That(target.mixedConcreteDependency, Is.EqualTo(singleDependency));
            CollectionAssert.AreEquivalent(dependencies, target.mixedConcreteList);
            Assert.That(target.mixedInterfaceDependency, Is.EqualTo(singleDependency));
        }
    }
}
