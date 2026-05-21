using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Plugins.Saneject.Editor.Data.Context;
using Tests.Saneject.Fixtures.Scripts;
using Tests.Saneject.Fixtures.Scripts.InjectionTargets;
using UnityEditor;
using UnityEngine;

namespace Tests.Saneject.Editor.Binding.LocatorMethods.AssetLocatorMethods.Special
{
    public class FromInstancesTests
    {
        [Test]
        public void FromInstances_TConcrete_InjectsToConcreteCollection()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope scope = scene.Add<TestScope>("Root 1");
            MultiPrefabAssetDependencyTarget target = scene.Add<MultiPrefabAssetDependencyTarget>("Root 1");
            TestPrefabAsset firstPrefab = TestPrefabAsset.Create("First Prefab Root", width: 1, depth: 1);
            TestPrefabAsset secondPrefab = TestPrefabAsset.Create("Second Prefab Root", width: 1, depth: 1);
            PrefabUtility.SaveAsPrefabAsset(firstPrefab.Root, firstPrefab.AssetPath);
            PrefabUtility.SaveAsPrefabAsset(secondPrefab.Root, secondPrefab.AssetPath);

            try
            {
                // Find dependencies
                GameObject[] dependencyPrefabs =
                {
                    AssetDatabase.LoadAssetAtPath<GameObject>(firstPrefab.AssetPath),
                    AssetDatabase.LoadAssetAtPath<GameObject>(secondPrefab.AssetPath)
                };

                // Bind
                scope.BindAssets<GameObject>().FromInstances(dependencyPrefabs);

                // Inject
                InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

                // Assert
                CollectionAssert.AllItemsAreNotNull(dependencyPrefabs);
                CollectionAssert.AreEquivalent(dependencyPrefabs, target.array);
                CollectionAssert.AreEquivalent(dependencyPrefabs, target.list);
            }
            finally
            {
                firstPrefab.Destroy();
                firstPrefab.DeleteAsset();
                secondPrefab.Destroy();
                secondPrefab.DeleteAsset();
            }
        }
    }
}
