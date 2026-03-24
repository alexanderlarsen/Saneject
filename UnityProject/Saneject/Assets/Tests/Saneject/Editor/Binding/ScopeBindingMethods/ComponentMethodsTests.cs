using System.Text.RegularExpressions;
using NUnit.Framework;
using Plugins.Saneject.Editor.Data.Context;
using Plugins.Saneject.Editor.Pipeline;
using Tests.Saneject.Fixtures.Scripts;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using Tests.Saneject.Fixtures.Scripts.InjectionTargets;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Saneject.Editor.Binding.ScopeBindingMethods
{
    public class ComponentMethodsTests
    {
        [Test]
        public void BindComponent_TConcrete_InjectsConcrete_NotInterface()
        {
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Missing binding")); // Interface target
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Injection complete"));

            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleConcreteComponentTarget concreteTarget = scene.Add<SingleConcreteComponentTarget>("Root 1");
            SingleInterfaceTarget interfaceTarget = scene.Add<SingleInterfaceTarget>("Root 1");
            ComponentDependency dependency = scene.Add<ComponentDependency>("Root 1");

            scope.BindComponent<ComponentDependency>().FromSelf();

            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            Assert.That(dependency, Is.Not.Null);
            Assert.That(concreteTarget.dependency, Is.Not.Null);
            Assert.That(concreteTarget.dependency, Is.EqualTo(dependency));
            Assert.That(interfaceTarget.dependency, Is.Null);
        }

        [Test]
        public void BindComponent_TInterface_InjectsInterface_NotConcrete()
        {
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Missing binding")); // Concrete target
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Injection complete"));

            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleConcreteComponentTarget concreteTarget = scene.Add<SingleConcreteComponentTarget>("Root 1");
            SingleInterfaceTarget interfaceTarget = scene.Add<SingleInterfaceTarget>("Root 1");
            ComponentDependency dependency = scene.Add<ComponentDependency>("Root 1");

            scope.BindComponent<IDependency>().FromSelf();

            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            Assert.That(dependency, Is.Not.Null);
            Assert.That(interfaceTarget.dependency, Is.Not.Null);
            Assert.That(interfaceTarget.dependency, Is.EqualTo(dependency));
            Assert.That(concreteTarget.dependency, Is.Null);
        }

        [Test]
        public void BindComponents_TConcrete_InjectsConcreteCollection_NotInterfaceCollection()
        {
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Missing binding")); // Interface target array
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Missing binding")); // Interface target list
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Injection complete"));

            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 5);
            TestScope scope = scene.Add<TestScope>("Root 1");
            MultiConcreteComponentTarget concreteTarget = scene.Add<MultiConcreteComponentTarget>("Root 1");
            MultiInterfaceTarget interfaceTarget = scene.Add<MultiInterfaceTarget>("Root 1");
            ComponentDependency[] dependencies = scene.AddToAllTransforms<ComponentDependency>();

            scope.BindComponents<ComponentDependency>().FromDescendants(includeSelf: true);

            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            CollectionAssert.IsNotEmpty(dependencies);
            CollectionAssert.AllItemsAreNotNull(dependencies);
            CollectionAssert.AllItemsAreUnique(dependencies);
            CollectionAssert.AllItemsAreInstancesOfType(dependencies, typeof(ComponentDependency));

            CollectionAssert.AreEquivalent(dependencies, concreteTarget.array);
            CollectionAssert.AreEquivalent(dependencies, concreteTarget.list);

            Assert.That(interfaceTarget.array, Is.Null);
            Assert.That(interfaceTarget.list, Is.Null);
        }

        [Test]
        public void BindComponents_TInterface_InjectsInterfaceCollection_NotConcreteCollection()
        {
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Missing binding")); // Concrete target array
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Missing binding")); // Concrete target list
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Injection complete"));

            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 5);
            TestScope scope = scene.Add<TestScope>("Root 1");
            MultiConcreteComponentTarget concreteTarget = scene.Add<MultiConcreteComponentTarget>("Root 1");
            MultiInterfaceTarget interfaceTarget = scene.Add<MultiInterfaceTarget>("Root 1");
            ComponentDependency[] dependencies = scene.AddToAllTransforms<ComponentDependency>();

            scope.BindComponents<IDependency>().FromDescendants(includeSelf: true);

            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            CollectionAssert.IsNotEmpty(dependencies);
            CollectionAssert.AllItemsAreNotNull(dependencies);
            CollectionAssert.AllItemsAreUnique(dependencies);
            CollectionAssert.AllItemsAreInstancesOfType(dependencies, typeof(ComponentDependency));

            CollectionAssert.AreEquivalent(dependencies, interfaceTarget.array);
            CollectionAssert.AreEquivalent(dependencies, interfaceTarget.list);

            Assert.That(concreteTarget.array, Is.Null);
            Assert.That(concreteTarget.list, Is.Null);
        }

        [Test]
        public void BindComponent_TInterfaceTConcrete_InjectsInterface_NotConcrete()
        {
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Missing binding")); // Concrete target
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Injection complete"));

            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleConcreteComponentTarget concreteTarget = scene.Add<SingleConcreteComponentTarget>("Root 1");
            SingleInterfaceTarget interfaceTarget = scene.Add<SingleInterfaceTarget>("Root 1");
            ComponentDependency dependency = scene.Add<ComponentDependency>("Root 1");

            scope.BindComponent<IDependency, ComponentDependency>().FromSelf();

            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            Assert.That(dependency, Is.Not.Null);
            Assert.That(interfaceTarget.dependency, Is.Not.Null);
            Assert.That(interfaceTarget.dependency, Is.InstanceOf<ComponentDependency>());
            Assert.That(interfaceTarget.dependency, Is.EqualTo(dependency));
            Assert.That(concreteTarget.dependency, Is.Null);
        }

        // TODO: Move to invalid bindings test
        [Test]
        public void BindComponent_TConcreteTConcrete_IsInvalid()
        {
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Invalid binding"));
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Injection complete"));

            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope scope = scene.Add<TestScope>("Root 1");

            scope.BindComponent<ComponentDependency, ComponentDependency>().FromSelf();

            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
        }

        [Test]
        public void BindComponents_TInterfaceTConcrete_InjectsInterfaceCollection_NotConcreteCollection()
        {
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Missing binding")); // Concrete target array
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Missing binding")); // Concrete target list
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Injection complete"));

            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 5);
            TestScope scope = scene.Add<TestScope>("Root 1");
            MultiConcreteComponentTarget concreteTarget = scene.Add<MultiConcreteComponentTarget>("Root 1");
            MultiInterfaceTarget interfaceTarget = scene.Add<MultiInterfaceTarget>("Root 1");
            ComponentDependency[] dependencies = scene.AddToAllTransforms<ComponentDependency>();

            scope.BindComponents<IDependency, ComponentDependency>().FromDescendants(includeSelf: true);

            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            CollectionAssert.IsNotEmpty(dependencies);
            CollectionAssert.AllItemsAreNotNull(dependencies);
            CollectionAssert.AllItemsAreUnique(dependencies);
            CollectionAssert.AllItemsAreInstancesOfType(dependencies, typeof(ComponentDependency));

            CollectionAssert.AreEquivalent(dependencies, interfaceTarget.array);
            CollectionAssert.AreEquivalent(dependencies, interfaceTarget.list);

            Assert.That(concreteTarget.array, Is.Null);
            Assert.That(concreteTarget.list, Is.Null);
        }

        // TODO: Move to invalid bindings test
        [Test]
        public void BindComponents_TConcreteTConcrete_IsInvalid()
        {
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Invalid binding"));
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Injection complete"));

            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 5);
            TestScope scope = scene.Add<TestScope>("Root 1");

            scope.BindComponents<ComponentDependency, ComponentDependency>().FromDescendants(includeSelf: true);

            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
        }
    }
}