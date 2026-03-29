using System;
using System.Reflection;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Plugins.Saneject.Editor.Menus.SanejectMenuItems;
using Plugins.Saneject.Runtime.Settings;
using Tests.Saneject.Fixtures.Scripts;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace Tests.Saneject.Editor.Editor.Menus
{
    public class SelectSameContextMenusTests
    {
        private bool originalUseContextIsolation;

        [SetUp]
        public void SetUp()
        {
            originalUseContextIsolation = ProjectSettings.UseContextIsolation;
        }

        [TearDown]
        public void TearDown()
        {
            ProjectSettings.UseContextIsolation = originalUseContextIsolation;
            Selection.objects = Array.Empty<Object>();

            TestPrefabAsset.CloseStage();
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        }

        [Test]
        public void SelectSameContextInScene_GivenContextIsolationOn_SelectsSameContextObjects()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 2, depth: 2);
            TestPrefabAsset prefab = TestPrefabAsset.Create("Prefab Root", width: 1, depth: 2);
            TestPrefabInstance prefabInstanceOne = prefab.Instantiate(scene.GetTransform("Root 1/Child 1"), "Prefab Instance 1");
            prefabInstanceOne.AddTransform("Prefab Root/Scene Child");
            TestPrefabInstance prefabInstanceTwo = prefab.Instantiate(scene.GetTransform("Root 1/Child 2"), "Prefab Instance 2");
            prefab.Destroy();
            MethodInfo selectSameContextInSceneMethod = typeof(SelectSameContextMenus).GetMethod
            (
                "SelectSameContextInScene",
                BindingFlags.NonPublic | BindingFlags.Static
            );

            try
            {
                // Select object
                Selection.objects = new Object[]
                {
                    prefabInstanceOne.GetTransform("Prefab Root/Child 1").gameObject
                };

                // Select same context
                ProjectSettings.UseContextIsolation = true;
                selectSameContextInSceneMethod.Invoke(null, new object[] { new MenuCommand(Selection.objects[0]) });

                // Assert
                Assert.That(selectSameContextInSceneMethod, Is.Not.Null);
                CollectionAssert.AreEquivalent(new Object[]
                {
                    prefabInstanceOne.GetTransform("Prefab Root").gameObject,
                    prefabInstanceOne.GetTransform("Prefab Root/Child 1").gameObject
                }, Selection.objects);
            }
            finally
            {
                prefabInstanceOne.Destroy();
                prefabInstanceTwo.Destroy();

                prefab.DeleteAsset();
            }
        }

        [Test]
        public void SelectSameContextInScene_GivenContextIsolationOff_SelectsAllSceneAndPrefabInstanceObjects()
        {
            // Expect log
            LogAssert.Expect(LogType.Log, new Regex("^Saneject: Context isolation is disabled in project settings\\. Selecting all GameObjects in the current scene\\.$"));

            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 2, depth: 2);
            TestPrefabAsset prefab = TestPrefabAsset.Create("Prefab Root", width: 1, depth: 2);
            TestPrefabInstance prefabInstanceOne = prefab.Instantiate(scene.GetTransform("Root 1/Child 1"), "Prefab Instance 1");
            prefabInstanceOne.AddTransform("Prefab Root/Scene Child");
            TestPrefabInstance prefabInstanceTwo = prefab.Instantiate(scene.GetTransform("Root 1/Child 2"), "Prefab Instance 2");
            prefab.Destroy();
            MethodInfo selectSameContextInSceneMethod = typeof(SelectSameContextMenus).GetMethod
            (
                "SelectSameContextInScene",
                BindingFlags.NonPublic | BindingFlags.Static
            );

            try
            {
                // Select object
                Selection.objects = new Object[]
                {
                    prefabInstanceOne.GetTransform("Prefab Root/Child 1").gameObject
                };

                // Select same context
                ProjectSettings.UseContextIsolation = false;
                selectSameContextInSceneMethod.Invoke(null, new object[] { new MenuCommand(Selection.objects[0]) });

                // Assert
                Assert.That(selectSameContextInSceneMethod, Is.Not.Null);
                CollectionAssert.AreEquivalent(new Object[]
                {
                    scene.GetTransform("Root 1").gameObject,
                    scene.GetTransform("Root 1/Child 1").gameObject,
                    scene.GetTransform("Root 1/Child 2").gameObject,
                    prefabInstanceOne.GetTransform("Prefab Root").gameObject,
                    prefabInstanceOne.GetTransform("Prefab Root/Child 1").gameObject,
                    prefabInstanceOne.GetTransform("Prefab Root/Scene Child").gameObject,
                    prefabInstanceTwo.GetTransform("Prefab Root").gameObject,
                    prefabInstanceTwo.GetTransform("Prefab Root/Child 1").gameObject
                }, Selection.objects);
            }
            finally
            {
                prefabInstanceOne.Destroy();
                prefabInstanceTwo.Destroy();

                prefab.DeleteAsset();
            }
        }

        [Test]
        public void SelectSameContextInHierarchy_GivenContextIsolationOn_SelectsMatchingContextsInSelectedHierarchy()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 2, depth: 2);
            TestPrefabAsset prefab = TestPrefabAsset.Create("Prefab Root", width: 1, depth: 2);
            TestPrefabInstance prefabInstanceOne = prefab.Instantiate(scene.GetTransform("Root 1/Child 1"), "Prefab Instance 1");
            prefabInstanceOne.AddTransform("Prefab Root/Scene Child");
            TestPrefabInstance prefabInstanceTwo = prefab.Instantiate(scene.GetTransform("Root 1/Child 2"), "Prefab Instance 2");
            prefab.Destroy();
            MethodInfo selectSameContextInHierarchyMethod = typeof(SelectSameContextMenus).GetMethod
            (
                "SelectSameContextInHierarchy",
                BindingFlags.NonPublic | BindingFlags.Static
            );

            try
            {
                // Select objects
                Selection.objects = new Object[]
                {
                    scene.GetTransform("Root 1/Child 1").gameObject,
                    prefabInstanceOne.GetTransform("Prefab Root/Child 1").gameObject
                };

                // Select same contexts
                ProjectSettings.UseContextIsolation = true;
                selectSameContextInHierarchyMethod.Invoke(null, new object[] { new MenuCommand(Selection.objects[0]) });

                // Assert
                Assert.That(selectSameContextInHierarchyMethod, Is.Not.Null);
                CollectionAssert.AreEquivalent(new Object[]
                {
                    scene.GetTransform("Root 1").gameObject,
                    scene.GetTransform("Root 1/Child 1").gameObject,
                    scene.GetTransform("Root 1/Child 2").gameObject,
                    prefabInstanceOne.GetTransform("Prefab Root").gameObject,
                    prefabInstanceOne.GetTransform("Prefab Root/Child 1").gameObject,
                    prefabInstanceOne.GetTransform("Prefab Root/Scene Child").gameObject
                }, Selection.objects);
            }
            finally
            {
                prefabInstanceOne.Destroy();
                prefabInstanceTwo.Destroy();

                prefab.DeleteAsset();
            }
        }

        [Test]
        public void SelectSameContextInHierarchy_GivenContextIsolationOff_SelectsAllObjectsInSelectedHierarchy()
        {
            // Expect log
            LogAssert.Expect(LogType.Log, new Regex("^Saneject: Context isolation is disabled in project settings\\. Selecting all GameObjects in the selected hierarchy\\.$"));

            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 2, depth: 2);
            TestPrefabAsset prefab = TestPrefabAsset.Create("Prefab Root", width: 1, depth: 2);
            TestPrefabInstance prefabInstanceOne = prefab.Instantiate(scene.GetTransform("Root 1/Child 1"), "Prefab Instance 1");
            prefabInstanceOne.AddTransform("Prefab Root/Scene Child");
            TestPrefabInstance prefabInstanceTwo = prefab.Instantiate(scene.GetTransform("Root 1/Child 2"), "Prefab Instance 2");
            prefab.Destroy();
            MethodInfo selectSameContextInHierarchyMethod = typeof(SelectSameContextMenus).GetMethod
            (
                "SelectSameContextInHierarchy",
                BindingFlags.NonPublic | BindingFlags.Static
            );

            try
            {
                // Select object
                Selection.objects = new Object[]
                {
                    scene.GetTransform("Root 1/Child 1").gameObject
                };

                // Select same hierarchy
                ProjectSettings.UseContextIsolation = false;
                selectSameContextInHierarchyMethod.Invoke(null, new object[] { new MenuCommand(Selection.objects[0]) });

                // Assert
                Assert.That(selectSameContextInHierarchyMethod, Is.Not.Null);
                CollectionAssert.AreEquivalent(new Object[]
                {
                    scene.GetTransform("Root 1").gameObject,
                    scene.GetTransform("Root 1/Child 1").gameObject,
                    scene.GetTransform("Root 1/Child 2").gameObject,
                    prefabInstanceOne.GetTransform("Prefab Root").gameObject,
                    prefabInstanceOne.GetTransform("Prefab Root/Child 1").gameObject,
                    prefabInstanceOne.GetTransform("Prefab Root/Scene Child").gameObject,
                    prefabInstanceTwo.GetTransform("Prefab Root").gameObject,
                    prefabInstanceTwo.GetTransform("Prefab Root/Child 1").gameObject
                }, Selection.objects);
            }
            finally
            {
                prefabInstanceOne.Destroy();
                prefabInstanceTwo.Destroy();

                prefab.DeleteAsset();
            }
        }

        [Test]
        public void SelectSameContextInHierarchy_GivenPrefabAssetAndPrefabInstanceSelection_SelectsMatchingContextsInSelectedHierarchy()
        {
            // Set up prefab assets
            TestPrefabAsset hostPrefab = TestPrefabAsset.Create("Host Root", width: 2, depth: 2);
            TestPrefabAsset nestedPrefab = TestPrefabAsset.Create("Nested Root", width: 1, depth: 2);
            TestPrefabInstance hostStage = hostPrefab.OpenStage();
            TestPrefabInstance nestedInstanceOne = nestedPrefab.Instantiate(hostStage.GetTransform("Host Root/Child 1"), "Nested Prefab 1");
            TestPrefabInstance nestedInstanceTwo = nestedPrefab.Instantiate(hostStage.GetTransform("Host Root/Child 2"), "Nested Prefab 2");
            MethodInfo selectSameContextInHierarchyMethod = typeof(SelectSameContextMenus).GetMethod
            (
                "SelectSameContextInHierarchy",
                BindingFlags.NonPublic | BindingFlags.Static
            );

            try
            {
                // Select objects
                Selection.objects = new Object[]
                {
                    hostStage.GetTransform("Host Root/Child 1").gameObject,
                    nestedInstanceOne.GetTransform("Nested Root/Child 1").gameObject
                };

                // Select same hierarchy
                ProjectSettings.UseContextIsolation = true;
                selectSameContextInHierarchyMethod.Invoke(null, new object[] { new MenuCommand(Selection.objects[0]) });

                // Assert
                Assert.That(selectSameContextInHierarchyMethod, Is.Not.Null);
                CollectionAssert.AreEquivalent(new Object[]
                {
                    hostStage.GetTransform("Host Root").gameObject,
                    hostStage.GetTransform("Host Root/Child 1").gameObject,
                    hostStage.GetTransform("Host Root/Child 2").gameObject,
                    nestedInstanceOne.GetTransform("Nested Root").gameObject,
                    nestedInstanceOne.GetTransform("Nested Root/Child 1").gameObject
                }, Selection.objects);
            }
            finally
            {
                nestedInstanceOne.Destroy();
                nestedInstanceTwo.Destroy();

                TestPrefabAsset.CloseStage();

                hostPrefab.Destroy();
                hostPrefab.DeleteAsset();

                nestedPrefab.Destroy();
                nestedPrefab.DeleteAsset();
            }
        }

        [Test]
        public void Validate_SelectSameContextInScene_GivenPrefabStageSelection_ReturnsFalse()
        {
            // Set up prefab asset
            TestPrefabAsset prefab = TestPrefabAsset.Create("Prefab Root", width: 1, depth: 1);
            TestPrefabInstance prefabStage = prefab.OpenStage();
            MethodInfo validateMethod = typeof(SelectSameContextMenus).GetMethod
            (
                "Validate_SelectSameContextInScene",
                BindingFlags.NonPublic | BindingFlags.Static
            );

            try
            {
                // Select object
                Selection.objects = new Object[] { prefabStage.Root };

                // Assert
                Assert.That(validateMethod, Is.Not.Null);
                Assert.That((bool)validateMethod.Invoke(null, null), Is.False);
            }
            finally
            {
                TestPrefabAsset.CloseStage();
                prefab.Destroy();
                prefab.DeleteAsset();
            }
        }

        [Test]
        public void Validate_SelectSameContextInScene_GivenPrefabInstanceSelection_ReturnsTrue()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 2);
            TestPrefabAsset prefab = TestPrefabAsset.Create("Prefab Root", width: 1, depth: 2);
            TestPrefabInstance prefabInstance = prefab.Instantiate(scene.GetTransform("Root 1"), "Prefab Instance");
            prefab.Destroy();
            MethodInfo validateMethod = typeof(SelectSameContextMenus).GetMethod
            (
                "Validate_SelectSameContextInScene",
                BindingFlags.NonPublic | BindingFlags.Static
            );

            try
            {
                // Select object
                Selection.objects = new Object[]
                {
                    prefabInstance.GetTransform("Prefab Root/Child 1").gameObject
                };

                // Assert
                Assert.That(validateMethod, Is.Not.Null);
                Assert.That((bool)validateMethod.Invoke(null, null), Is.True);
            }
            finally
            {
                prefabInstance.Destroy();

                prefab.DeleteAsset();
            }
        }

        [Test]
        public void Validate_SelectSameContextInHierarchy_GivenSceneObjectSelection_ReturnsTrue()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            MethodInfo validateMethod = typeof(SelectSameContextMenus).GetMethod
            (
                "Validate_SelectSameContextInHierarchy",
                BindingFlags.NonPublic | BindingFlags.Static
            );

            // Select object
            Selection.objects = new Object[] { scene.GetTransform("Root 1").gameObject };

            // Assert
            Assert.That(validateMethod, Is.Not.Null);
            Assert.That((bool)validateMethod.Invoke(null, null), Is.True);
        }

        [Test]
        public void Validate_SelectSameContextInHierarchy_GivenPrefabStageSelection_ReturnsTrue()
        {
            // Set up prefab asset
            TestPrefabAsset prefab = TestPrefabAsset.Create("Prefab Root", width: 1, depth: 1);
            TestPrefabInstance prefabStage = prefab.OpenStage();
            MethodInfo validateMethod = typeof(SelectSameContextMenus).GetMethod
            (
                "Validate_SelectSameContextInHierarchy",
                BindingFlags.NonPublic | BindingFlags.Static
            );

            try
            {
                // Select object
                Selection.objects = new Object[] { prefabStage.Root };

                // Assert
                Assert.That(validateMethod, Is.Not.Null);
                Assert.That((bool)validateMethod.Invoke(null, null), Is.True);
            }
            finally
            {
                TestPrefabAsset.CloseStage();
                prefab.Destroy();
                prefab.DeleteAsset();
            }
        }
    }
}
