using NUnit.Framework;
using Plugins.Saneject.Editor.Data.Context;
using Plugins.Saneject.Editor.Pipeline;
using Tests.Saneject.Fixtures.Scripts;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using Tests.Saneject.Fixtures.Scripts.InjectionTargets;
using UnityEngine;

namespace Tests.Saneject.Editor.Binding.Locators.AssetLocators.FromFolder
{
    public class FromFolderTests
    {
        [Test]
        public void FromFolder_TConcrete_InjectsToConcreteField()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleConcreteAssetTarget target = scene.Add<SingleConcreteAssetTarget>("Root 1");

            // Find dependency
            AssetDependency dependency = Resources.Load<AssetDependency>("AssetDependency 1");

            // Bind
            scope.BindAsset<AssetDependency>().FromFolder("Assets/Tests/Saneject/Fixtures/Resources");

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromFolder_TInterface_InjectsToInterfaceField()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleInterfaceTarget target = scene.Add<SingleInterfaceTarget>("Root 1");

            // Find dependency
            AssetDependency dependency = Resources.Load<AssetDependency>("AssetDependency 1");

            // Bind
            scope.BindAsset<IDependency, AssetDependency>().FromFolder("Assets/Tests/Saneject/Fixtures/Resources");

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromFolder_TConcrete_InjectsToConcreteCollection()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope scope = scene.Add<TestScope>("Root 1");
            MultiConcreteAssetTarget target = scene.Add<MultiConcreteAssetTarget>("Root 1");

            // Find dependencies
            AssetDependency[] dependencies = Resources.LoadAll<AssetDependency>("");

            // Bind
            scope.BindAssets<AssetDependency>().FromFolder("Assets/Tests/Saneject/Fixtures/Resources");

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            CollectionAssert.IsNotEmpty(dependencies);
            CollectionAssert.AreEquivalent(dependencies, target.array);
            CollectionAssert.AreEquivalent(dependencies, target.list);
        }
    }
}