using NUnit.Framework;
using Plugins.Saneject.Editor.Data.Context;
using Tests.Saneject.Fixtures.Scripts;
using UnityEditor;
using UnityEngine;

namespace Tests.Saneject.Editor.Context
{
    public class ContextIdentityTests
    {
        [Test]
        public void SceneObject_InScene_SetsExpectedContextIdentity()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 2);
            GameObject sceneObject = scene.GetTransform("Root 1/Child 1").gameObject;
            ContextIdentity identity = new(sceneObject);

            // Assert
            Assert.That(sceneObject, Is.Not.Null);
            Assert.That(identity.Type, Is.EqualTo(ContextType.SceneObject));
            Assert.That(identity.Id, Is.EqualTo(sceneObject.gameObject.scene.handle));
            Assert.That(identity.ContainerType, Is.EqualTo(ContextType.SceneObject));
            Assert.That(identity.ContainerId, Is.EqualTo(sceneObject.gameObject.scene.handle));
            Assert.That(identity.IsPrefab, Is.False);
        }

        [Test]
        public void PrefabInstance_InScene_SetsExpectedContextIdentity()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 2);
            TestPrefabAsset prefab = TestPrefabAsset.Create("Prefab Root", width: 1, depth: 2);
            TestPrefabInstance prefabInstance = prefab.Instantiate(scene.GetTransform("Root 1"), "Prefab Instance");

            try
            {
                // Capture context identity
                Transform prefabObject = prefabInstance.GetTransform("Prefab Root/Child 1");
                ContextIdentity identity = new(prefabObject);

                // Assert
                Assert.That(prefabObject, Is.Not.Null);
                Assert.That(identity.Type, Is.EqualTo(ContextType.PrefabInstance));
                Assert.That(identity.Id, Is.EqualTo(prefabInstance.Root.GetInstanceID()));
                Assert.That(identity.ContainerType, Is.EqualTo(ContextType.SceneObject));
                Assert.That(identity.ContainerId, Is.EqualTo(prefabObject.gameObject.scene.handle));
                Assert.That(identity.IsPrefab, Is.True);
            }
            finally
            {
                prefabInstance?.Destroy();
                prefab.Destroy();
                prefab.DeleteAsset();
            }
        }

        [Test]
        public void PrefabAssetObject_InPrefabAsset_SetsExpectedContextIdentity()
        {
            // Set up prefab asset
            TestPrefabAsset prefab = TestPrefabAsset.Create("Prefab Root", width: 1, depth: 2);
            TestPrefabInstance prefabStage = prefab.OpenStage();

            try
            {
                // Capture context identity
                Transform prefabObject = prefabStage.GetTransform("Prefab Root/Child 1");
                ContextIdentity identity = new(prefabObject);

                // Assert
                Assert.That(prefabObject, Is.Not.Null);
                Assert.That(identity.Type, Is.EqualTo(ContextType.PrefabAsset));
                Assert.That(identity.Id, Is.EqualTo(prefabStage.Root.GetInstanceID()));
                Assert.That(identity.ContainerType, Is.EqualTo(ContextType.PrefabAsset));
                Assert.That(identity.ContainerId, Is.EqualTo(prefabStage.Root.GetInstanceID()));
                Assert.That(identity.IsPrefab, Is.True);
            }
            finally
            {
                TestPrefabAsset.CloseStage();
                prefab.Destroy();
                prefab.DeleteAsset();
            }
        }

        [Test]
        public void PrefabAssetObject_LoadedFromAssetDatabase_SetsExpectedContextIdentity()
        {
            // Set up prefab asset
            TestPrefabAsset prefab = TestPrefabAsset.Create("Prefab Root", width: 1, depth: 2);
            PrefabUtility.SaveAsPrefabAsset(prefab.Root, prefab.AssetPath);

            try
            {
                // Capture context identity
                GameObject prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(prefab.AssetPath);
                Transform prefabObject = prefabAsset.transform.Find("Child 1");
                ContextIdentity identity = new(prefabObject);

                // Assert
                Assert.That(prefabAsset, Is.Not.Null);
                Assert.That(prefabObject, Is.Not.Null);
                Assert.That(identity.Type, Is.EqualTo(ContextType.PrefabAsset));
                Assert.That(identity.Id, Is.EqualTo(prefabAsset.GetInstanceID()));
                Assert.That(identity.ContainerType, Is.EqualTo(ContextType.PrefabAsset));
                Assert.That(identity.ContainerId, Is.EqualTo(prefabAsset.GetInstanceID()));
                Assert.That(identity.IsPrefab, Is.True);
            }
            finally
            {
                prefab.Destroy();
                prefab.DeleteAsset();
            }
        }

        [Test]
        public void PrefabInstance_InPrefabAsset_SetsExpectedContextIdentity()
        {
            // Set up prefab assets
            TestPrefabAsset hostPrefab = TestPrefabAsset.Create("Host Root", width: 1, depth: 1);
            TestPrefabAsset nestedPrefab = TestPrefabAsset.Create("Nested Root", width: 1, depth: 2);
            TestPrefabInstance hostStage = hostPrefab.OpenStage();
            TestPrefabInstance nestedInstance = nestedPrefab.Instantiate(hostStage.GetTransform("Host Root"), "Nested Prefab");

            try
            {
                // Capture context identity
                Transform prefabObject = nestedInstance.GetTransform("Nested Root/Child 1");
                ContextIdentity identity = new(prefabObject);

                // Assert
                Assert.That(prefabObject, Is.Not.Null);
                Assert.That(identity.Type, Is.EqualTo(ContextType.PrefabInstance));
                Assert.That(identity.Id, Is.EqualTo(nestedInstance.Root.GetInstanceID()));
                Assert.That(identity.ContainerType, Is.EqualTo(ContextType.PrefabAsset));
                Assert.That(identity.ContainerId, Is.EqualTo(hostStage.Root.GetInstanceID()));
                Assert.That(identity.IsPrefab, Is.True);
            }
            finally
            {
                nestedInstance?.Destroy();
                TestPrefabAsset.CloseStage();
                hostPrefab.Destroy();
                hostPrefab.DeleteAsset();
                nestedPrefab.Destroy();
                nestedPrefab.DeleteAsset();
            }
        }
    }
}