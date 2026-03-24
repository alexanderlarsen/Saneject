using NUnit.Framework;
using Plugins.Saneject.Editor.Data.Context;
using Plugins.Saneject.Editor.Pipeline;
using Tests.Saneject.Fixtures.Scripts;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using Tests.Saneject.Fixtures.Scripts.InjectionTargets;
using UnityEngine;

namespace Tests.Saneject.Editor.Binding.Filters.AssetFilters
{
    public class WhereTests
    {
        [Test]
        public void Where_TConcrete_InjectsFilteredAssetToConcreteField()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleConcreteAssetTarget target = scene.Add<SingleConcreteAssetTarget>("Root 1");

            // Find dependency
            AssetDependency dependency = Resources.Load<AssetDependency>("AssetDependency 2");

            // Bind
            scope.BindAsset<AssetDependency>()
                .FromFolder("Assets/Tests/Saneject/Fixtures/Resources")
                .Where(asset => asset.name == "AssetDependency 2");

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void Where_TInterfaceTConcrete_InjectsFilteredAssetToInterfaceField()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleInterfaceTarget target = scene.Add<SingleInterfaceTarget>("Root 1");

            // Find dependency
            AssetDependency dependency = Resources.Load<AssetDependency>("AssetDependency 3");

            // Bind
            scope.BindAsset<IDependency, AssetDependency>()
                .FromFolder("Assets/Tests/Saneject/Fixtures/Resources")
                .Where(asset => asset.name == "AssetDependency 3");

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void Where_TConcrete_InjectsFilteredAssetsToConcreteCollection()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope scope = scene.Add<TestScope>("Root 1");
            MultiConcreteAssetTarget target = scene.Add<MultiConcreteAssetTarget>("Root 1");

            // Find dependency
            AssetDependency[] dependencies =
            {
                Resources.Load<AssetDependency>("AssetDependency 1"),
                Resources.Load<AssetDependency>("AssetDependency 3")
            };

            // Bind
            scope.BindAssets<AssetDependency>()
                .FromFolder("Assets/Tests/Saneject/Fixtures/Resources")
                .Where(asset => asset.name != "AssetDependency 2");

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            CollectionAssert.IsNotEmpty(dependencies);
            CollectionAssert.AllItemsAreNotNull(dependencies);
            CollectionAssert.AllItemsAreUnique(dependencies);
            CollectionAssert.AllItemsAreInstancesOfType(dependencies, typeof(AssetDependency));

            CollectionAssert.AreEquivalent(dependencies, target.array);
            CollectionAssert.AreEquivalent(dependencies, target.list);
        }

        [Test]
        public void Where_TInterfaceTConcrete_InjectsFilteredAssetsToInterfaceCollection()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope scope = scene.Add<TestScope>("Root 1");
            MultiInterfaceTarget target = scene.Add<MultiInterfaceTarget>("Root 1");

            // Find dependency
            AssetDependency[] dependencies =
            {
                Resources.Load<AssetDependency>("AssetDependency 2"),
                Resources.Load<AssetDependency>("AssetDependency 3")
            };

            // Bind
            scope.BindAssets<IDependency, AssetDependency>()
                .FromFolder("Assets/Tests/Saneject/Fixtures/Resources")
                .Where(asset => asset.name != "AssetDependency 1");

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            CollectionAssert.IsNotEmpty(dependencies);
            CollectionAssert.AllItemsAreNotNull(dependencies);
            CollectionAssert.AllItemsAreUnique(dependencies);
            CollectionAssert.AllItemsAreInstancesOfType(dependencies, typeof(AssetDependency));

            CollectionAssert.AreEquivalent(dependencies, target.array);
            CollectionAssert.AreEquivalent(dependencies, target.list);
        }
    }
}
