using System.Text.RegularExpressions;
using NUnit.Framework;
using Plugins.Saneject.Editor.Data.Context;
using Plugins.Saneject.Editor.Pipeline;
using Tests.Saneject.Fixtures.Scripts;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using Tests.Saneject.Fixtures.Scripts.InjectionTargets;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Saneject.Editor.Injection
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

        [Test]
        public void Inject_MultipleInjectMethods_InvokesAll()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 2);
            TestScope scope = scene.Add<TestScope>("Root 1");
            MultipleMethodTarget target = scene.Add<MultipleMethodTarget>("Root 1");

            // Find dependency
            ComponentDependency dependency = scene.Add<ComponentDependency>("Root 1");

            // Bind
            scope.BindComponent<ComponentDependency>().FromSelf();
            scope.BindComponent<IDependency>().FromSelf();

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.firstMethodCalled, Is.True);
            Assert.That(target.secondMethodCalled, Is.True);
            Assert.That(target.firstDependency, Is.EqualTo(dependency));
            Assert.That(target.secondDependency, Is.EqualTo(dependency));
        }

        [Test]
        public void Inject_PrivateAndProtectedMethods_InvokesAll()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 2);
            TestScope scope = scene.Add<TestScope>("Root 1");
            NonPublicMethodTarget target = scene.Add<NonPublicMethodTarget>("Root 1");

            // Find dependency
            ComponentDependency dependency = scene.Add<ComponentDependency>("Root 1");

            // Bind
            scope.BindComponent<ComponentDependency>().FromSelf();

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.protectedMethodCalled, Is.True);
            Assert.That(target.privateMethodCalled, Is.True);
        }

        [Test]
        public void Inject_FieldAndMethodMembers_InjectsBoth()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 2);
            TestScope scope = scene.Add<TestScope>("Root 1");
            FieldAndMethodTarget target = scene.Add<FieldAndMethodTarget>("Root 1");

            // Find dependency
            ComponentDependency dependency = scene.Add<ComponentDependency>("Root 1");

            // Bind
            scope.BindComponent<ComponentDependency>().FromSelf();

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.fieldDependency, Is.EqualTo(dependency));
            Assert.That(target.methodDependency, Is.EqualTo(dependency));
        }

        [Test]
        public void Inject_WHEN_OneMethodHasMissingParameters_InvokesOnlyResolvableMethods()
        {
            // Expect logs
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Missing binding"));
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Injection complete"));

            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 2);
            TestScope scope = scene.Add<TestScope>("Root 1");
            MixedResolutionMethodTarget target = scene.Add<MixedResolutionMethodTarget>("Root 1");

            // Find dependency
            ComponentDependency dependency = scene.Add<ComponentDependency>("Root 1");

            // Bind
            scope.BindComponent<ComponentDependency>().FromSelf();

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.concreteMethodCalled, Is.True);
            Assert.That(target.concreteDependency, Is.EqualTo(dependency));
            Assert.That(target.mixedMethodCalled, Is.False);
            Assert.That(target.mixedConcreteDependency, Is.Null);
            Assert.That(target.mixedInterfaceDependency, Is.Null);
        }

        [Test]
        public void Inject_WHEN_AnyParameterIsMissing_DoesNotInvokeMethod()
        {
            // Expect logs
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Missing binding"));
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Injection complete"));

            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 2);
            TestScope scope = scene.Add<TestScope>("Root 1");
            PartialMethodTarget target = scene.Add<PartialMethodTarget>("Root 1");

            // Find dependency
            ComponentDependency dependency = scene.Add<ComponentDependency>("Root 1");

            // Bind
            scope.BindComponent<ComponentDependency>().FromSelf();

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.methodCalled, Is.False);
            Assert.That(target.dependency, Is.Null);
            Assert.That(target.interfaceDependency, Is.Null);
        }
    }
}
