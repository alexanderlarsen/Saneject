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
    public class AssetMethodsTests
    {
        [Test]
        public void BindAsset_TConcrete_InjectsConcrete_NotInterface()
        {
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Missing binding")); // Interface target
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Injection complete"));

            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleConcreteAssetTarget concreteTarget = scene.Add<SingleConcreteAssetTarget>("Root 1");
            SingleInterfaceTarget interfaceTarget = scene.Add<SingleInterfaceTarget>("Root 1");
            AssetDependency dependency = Resources.Load<AssetDependency>("AssetDependency 1");

            scope.BindAsset<AssetDependency>().FromAssetLoad("Assets/Tests/Saneject/Fixtures/Resources/AssetDependency 1.asset");

            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            Assert.That(dependency, Is.Not.Null);
            Assert.That(concreteTarget.dependency, Is.Not.Null);
            Assert.That(concreteTarget.dependency, Is.EqualTo(dependency));
            Assert.That(interfaceTarget.dependency, Is.Null);
        }

        [Test]
        public void BindMultipleAssets_TConcrete_InjectsConcreteCollection_NotInterfaceCollection()
        {
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Missing binding")); // Interface target array
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Missing binding")); // Interface target list
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Injection complete"));

            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 5);
            TestScope scope = scene.Add<TestScope>("Root 1");
            MultiConcreteAssetTarget concreteTarget = scene.Add<MultiConcreteAssetTarget>("Root 1");
            MultiInterfaceTarget interfaceTarget = scene.Add<MultiInterfaceTarget>("Root 1");
            AssetDependency[] dependencies = Resources.LoadAll<AssetDependency>("");

            scope.BindAssets<AssetDependency>().FromFolder("Assets/Tests/Saneject/Fixtures/Resources");

            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            CollectionAssert.IsNotEmpty(dependencies);
            CollectionAssert.AllItemsAreNotNull(dependencies);
            CollectionAssert.AllItemsAreUnique(dependencies);
            CollectionAssert.AllItemsAreInstancesOfType(dependencies, typeof(AssetDependency));

            CollectionAssert.AreEquivalent(dependencies, concreteTarget.array);
            CollectionAssert.AreEquivalent(dependencies, concreteTarget.list);

            Assert.That(interfaceTarget.array, Is.Null);
            Assert.That(interfaceTarget.list, Is.Null);
        }

        [Test]
        public void BindAsset_TInterfaceTConcrete_InjectsInterface_NotConcrete()
        {
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Missing binding")); // Concrete target
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Injection complete"));

            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleConcreteAssetTarget concreteTarget = scene.Add<SingleConcreteAssetTarget>("Root 1");
            SingleInterfaceTarget interfaceTarget = scene.Add<SingleInterfaceTarget>("Root 1");
            AssetDependency dependency = Resources.Load<AssetDependency>("AssetDependency 1");

            scope.BindAsset<IDependency, AssetDependency>().FromAssetLoad("Assets/Tests/Saneject/Fixtures/Resources/AssetDependency 1.asset");

            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            Assert.That(dependency, Is.Not.Null);
            Assert.That(interfaceTarget.dependency, Is.Not.Null);
            Assert.That(interfaceTarget.dependency, Is.InstanceOf<AssetDependency>());
            Assert.That(interfaceTarget.dependency, Is.EqualTo(dependency));
            Assert.That(concreteTarget.dependency, Is.Null);
        }
        
        // TODO: Move to invalid bindings test
        [Test]
        public void BindAsset_TConcreteTConcrete_IsInvalid()
        {
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Invalid binding"));
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Injection complete"));

            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope scope = scene.Add<TestScope>("Root 1");

            scope.BindAsset<AssetDependency, AssetDependency>().FromAssetLoad("Assets/Tests/Saneject/Fixtures/Resources/AssetDependency 1.asset");

            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
        }
        
        [Test]
        public void BindMultipleAssets_TInterfaceTConcrete_InjectsInterfaceCollection_NotConcreteCollection()
        {
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Missing binding")); // Concrete target array
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Missing binding")); // Concrete target list
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Injection complete"));

            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 5);
            TestScope scope = scene.Add<TestScope>("Root 1");
            MultiConcreteAssetTarget concreteTarget = scene.Add<MultiConcreteAssetTarget>("Root 1");
            MultiInterfaceTarget interfaceTarget = scene.Add<MultiInterfaceTarget>("Root 1");
            AssetDependency[] dependencies = Resources.LoadAll<AssetDependency>("");

            scope.BindAssets<IDependency, AssetDependency>().FromFolder("Assets/Tests/Saneject/Fixtures/Resources");

            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            CollectionAssert.IsNotEmpty(dependencies);
            CollectionAssert.AllItemsAreNotNull(dependencies);
            CollectionAssert.AllItemsAreUnique(dependencies);
            CollectionAssert.AllItemsAreInstancesOfType(dependencies, typeof(AssetDependency));

            CollectionAssert.AreEquivalent(dependencies, interfaceTarget.array);
            CollectionAssert.AreEquivalent(dependencies, interfaceTarget.list);

            Assert.That(concreteTarget.array, Is.Null);
            Assert.That(concreteTarget.list, Is.Null);
        }
        
        // TODO: Move to invalid bindings test
        [Test]
        public void BindMultipleAssets_TConcreteTConcrete_IsInvalid()
        {
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Invalid binding"));
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Injection complete"));

            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 5);
            TestScope scope = scene.Add<TestScope>("Root 1");

            scope.BindAssets<AssetDependency, AssetDependency>().FromFolder("Assets/Tests/Saneject/Fixtures/Resources");

            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
        }
    }
}