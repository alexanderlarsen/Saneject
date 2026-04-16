using NUnit.Framework;
using Plugins.Saneject.Editor.Data.Context;
using Tests.Saneject.Fixtures.Scripts;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Tests.Saneject.Editor.Context
{
    public class ContextIdentityEqualityTests
    {
        [Test]
        public void SceneA_SceneA_HaveEqual_ContextIdentities()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 2, depth: 2);

            // Capture context identities
            ContextIdentity firstIdentity = new(scene.GetTransform("Root 1"));
            ContextIdentity secondIdentity = new(scene.GetTransform("Root 1/Child 2"));

            // Assert
            Assert.That(firstIdentity.Type, Is.EqualTo(ContextType.SceneObject));
            Assert.That(secondIdentity.Type, Is.EqualTo(ContextType.SceneObject));
            Assert.That(firstIdentity.IsPrefab, Is.False);
            Assert.That(secondIdentity.IsPrefab, Is.False);
            Assert.That(firstIdentity, Is.EqualTo(secondIdentity));
            Assert.That(firstIdentity.Id, Is.EqualTo(secondIdentity.Id));
        }

        [Test]
        public void SceneA_SceneB_DoNotHaveEqual_ContextIdentities()
        {
            // Set up scenes
            TestScene sceneA = TestScene.Create(roots: 1, width: 1, depth: 2);
            sceneA.SaveToDisk();
            TestScene sceneB = TestScene.Create(roots: 1, width: 1, depth: 2, newSceneMode: NewSceneMode.Additive);
            sceneB.SaveToDisk();

            try
            {
                // Capture context identities
                ContextIdentity firstIdentity = new(sceneA.GetTransform("Root 1/Child 1"));
                ContextIdentity secondIdentity = new(sceneB.GetTransform("Root 1/Child 1"));

                // Assert
                Assert.That(firstIdentity.Type, Is.EqualTo(ContextType.SceneObject));
                Assert.That(secondIdentity.Type, Is.EqualTo(ContextType.SceneObject));
                Assert.That(firstIdentity.IsPrefab, Is.False);
                Assert.That(secondIdentity.IsPrefab, Is.False);
                Assert.That(firstIdentity, Is.Not.EqualTo(secondIdentity));
                Assert.That(firstIdentity.Id, Is.Not.EqualTo(secondIdentity.Id));
            }
            finally
            {
                EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                sceneA.DeleteFromDisk();
                sceneB.DeleteFromDisk();
            }
        }

        [Test]
        public void PrefabInstanceA_PrefabInstanceA_HaveEqual_ContextIdentities()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 2);
            TestPrefabAsset prefab = TestPrefabAsset.Create("Prefab Root", width: 1, depth: 2);
            TestPrefabInstance prefabInstance = prefab.Instantiate(scene.GetTransform("Root 1"), "Prefab Instance");

            try
            {
                // Capture context identities
                ContextIdentity firstIdentity = new(prefabInstance.GetTransform("Prefab Root"));
                ContextIdentity secondIdentity = new(prefabInstance.GetTransform("Prefab Root/Child 1"));

                // Assert
                Assert.That(firstIdentity.Type, Is.EqualTo(ContextType.PrefabInstance));
                Assert.That(secondIdentity.Type, Is.EqualTo(ContextType.PrefabInstance));
                Assert.That(firstIdentity.IsPrefab, Is.True);
                Assert.That(secondIdentity.IsPrefab, Is.True);
                Assert.That(firstIdentity, Is.EqualTo(secondIdentity));
                Assert.That(firstIdentity.Id, Is.EqualTo(secondIdentity.Id));
            }
            finally
            {
                prefabInstance?.Destroy();

                prefab.Destroy();
                prefab.DeleteAsset();
            }
        }

        [Test]
        public void PrefabInstanceA_PrefabInstanceB_DoNotHaveEqual_ContextIdentities()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 2);
            TestPrefabAsset prefab = TestPrefabAsset.Create("Prefab Root", width: 1, depth: 2);
            TestPrefabInstance prefabInstanceOne = prefab.Instantiate(scene.GetTransform("Root 1"), "Prefab Instance 1");
            TestPrefabInstance prefabInstanceTwo = prefab.Instantiate(scene.GetTransform("Root 1"), "Prefab Instance 2");

            try
            {
                // Capture context identities
                ContextIdentity firstIdentity = new(prefabInstanceOne.GetTransform("Prefab Root/Child 1"));
                ContextIdentity secondIdentity = new(prefabInstanceTwo.GetTransform("Prefab Root/Child 1"));

                // Assert
                Assert.That(firstIdentity.Type, Is.EqualTo(ContextType.PrefabInstance));
                Assert.That(secondIdentity.Type, Is.EqualTo(ContextType.PrefabInstance));
                Assert.That(firstIdentity.IsPrefab, Is.True);
                Assert.That(secondIdentity.IsPrefab, Is.True);
                Assert.That(firstIdentity, Is.Not.EqualTo(secondIdentity));
                Assert.That(firstIdentity.Id, Is.Not.EqualTo(secondIdentity.Id));
            }
            finally
            {
                prefabInstanceOne?.Destroy();
                prefabInstanceTwo?.Destroy();

                prefab.Destroy();
                prefab.DeleteAsset();
            }
        }

        [Test]
        public void PrefabAssetA_PrefabAssetA_HaveEqual_ContextIdentities()
        {
            // Set up prefab asset
            TestPrefabAsset prefab = TestPrefabAsset.Create("Prefab Root", width: 1, depth: 2);
            TestPrefabInstance prefabStage = prefab.OpenStage();

            try
            {
                // Capture context identities
                ContextIdentity firstIdentity = new(prefabStage.GetTransform("Prefab Root"));
                ContextIdentity secondIdentity = new(prefabStage.GetTransform("Prefab Root/Child 1"));

                // Assert
                Assert.That(firstIdentity.Type, Is.EqualTo(ContextType.PrefabAsset));
                Assert.That(secondIdentity.Type, Is.EqualTo(ContextType.PrefabAsset));
                Assert.That(firstIdentity.IsPrefab, Is.True);
                Assert.That(secondIdentity.IsPrefab, Is.True);
                Assert.That(firstIdentity, Is.EqualTo(secondIdentity));
                Assert.That(firstIdentity.Id, Is.EqualTo(secondIdentity.Id));
            }
            finally
            {
                TestPrefabAsset.CloseStage();

                prefab.Destroy();
                prefab.DeleteAsset();
            }
        }

        [Test]
        public void PrefabAssetA_PrefabAssetB_DoNotHaveEqual_ContextIdentities()
        {
            // Set up prefab assets
            TestPrefabAsset prefabOne = TestPrefabAsset.Create("Prefab Root A", width: 1, depth: 2);
            TestPrefabAsset prefabTwo = TestPrefabAsset.Create("Prefab Root B", width: 1, depth: 2);
            PrefabUtility.SaveAsPrefabAsset(prefabOne.Root, prefabOne.AssetPath);
            PrefabUtility.SaveAsPrefabAsset(prefabTwo.Root, prefabTwo.AssetPath);

            try
            {
                // Capture context identities
                GameObject prefabAssetOne = AssetDatabase.LoadAssetAtPath<GameObject>(prefabOne.AssetPath);
                GameObject prefabAssetTwo = AssetDatabase.LoadAssetAtPath<GameObject>(prefabTwo.AssetPath);
                ContextIdentity firstIdentity = new(prefabAssetOne.transform.GetChild(0));
                ContextIdentity secondIdentity = new(prefabAssetTwo.transform.GetChild(0));

                // Assert
                Assert.That(firstIdentity.Type, Is.EqualTo(ContextType.PrefabAsset));
                Assert.That(secondIdentity.Type, Is.EqualTo(ContextType.PrefabAsset));
                Assert.That(firstIdentity.IsPrefab, Is.True);
                Assert.That(secondIdentity.IsPrefab, Is.True);
                Assert.That(firstIdentity, Is.Not.EqualTo(secondIdentity));
                Assert.That(firstIdentity.Id, Is.Not.EqualTo(secondIdentity.Id));
            }
            finally
            {
                prefabOne.Destroy();
                prefabOne.DeleteAsset();

                prefabTwo.Destroy();
                prefabTwo.DeleteAsset();
            }
        }

        [Test]
        public void Scene_PrefabInstance_DoNotHaveEqual_ContextIdentities()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 2);
            TestPrefabAsset prefab = TestPrefabAsset.Create("Prefab Root", width: 1, depth: 2);
            TestPrefabInstance prefabInstance = prefab.Instantiate(scene.GetTransform("Root 1"), "Prefab Instance");

            try
            {
                // Capture context identities
                ContextIdentity sceneIdentity = new(scene.GetTransform("Root 1/Child 1"));
                ContextIdentity prefabInstanceIdentity = new(prefabInstance.GetTransform("Prefab Root/Child 1"));

                // Assert
                Assert.That(sceneIdentity.Type, Is.EqualTo(ContextType.SceneObject));
                Assert.That(prefabInstanceIdentity.Type, Is.EqualTo(ContextType.PrefabInstance));
                Assert.That(sceneIdentity, Is.Not.EqualTo(prefabInstanceIdentity));
            }
            finally
            {
                prefabInstance?.Destroy();

                prefab.Destroy();
                prefab.DeleteAsset();
            }
        }

        [Test]
        public void Scene_PrefabAsset_DoNotHaveEqual_ContextIdentities()
        {
            // Set up scene and prefab asset
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 2);
            TestPrefabAsset prefab = TestPrefabAsset.Create("Prefab Root", width: 1, depth: 2);
            PrefabUtility.SaveAsPrefabAsset(prefab.Root, prefab.AssetPath);

            try
            {
                // Capture context identities
                GameObject prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(prefab.AssetPath);
                ContextIdentity sceneIdentity = new(scene.GetTransform("Root 1/Child 1"));
                ContextIdentity prefabAssetIdentity = new(prefabAsset.transform.GetChild(0));

                // Assert
                Assert.That(sceneIdentity.Type, Is.EqualTo(ContextType.SceneObject));
                Assert.That(prefabAssetIdentity.Type, Is.EqualTo(ContextType.PrefabAsset));
                Assert.That(sceneIdentity, Is.Not.EqualTo(prefabAssetIdentity));
            }
            finally
            {
                prefab.Destroy();
                prefab.DeleteAsset();
            }
        }

        [Test]
        public void PrefabInstance_PrefabAsset_DoNotHaveEqual_ContextIdentities()
        {
            // Set up scene and prefab asset
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 2);
            TestPrefabAsset prefab = TestPrefabAsset.Create("Prefab Root", width: 1, depth: 2);
            TestPrefabInstance prefabInstance = prefab.Instantiate(scene.GetTransform("Root 1"), "Prefab Instance");

            try
            {
                // Capture context identities
                GameObject prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(prefab.AssetPath);
                ContextIdentity prefabInstanceIdentity = new(prefabInstance.GetTransform("Prefab Root/Child 1"));
                ContextIdentity prefabAssetIdentity = new(prefabAsset.transform.GetChild(0));

                // Assert
                Assert.That(prefabInstanceIdentity.Type, Is.EqualTo(ContextType.PrefabInstance));
                Assert.That(prefabAssetIdentity.Type, Is.EqualTo(ContextType.PrefabAsset));
                Assert.That(prefabInstanceIdentity, Is.Not.EqualTo(prefabAssetIdentity));
            }
            finally
            {
                prefabInstance?.Destroy();

                prefab.Destroy();
                prefab.DeleteAsset();
            }
        }
    }
}