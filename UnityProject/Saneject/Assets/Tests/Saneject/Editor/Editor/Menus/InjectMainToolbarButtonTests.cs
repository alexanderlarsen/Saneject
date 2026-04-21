using System;
using NUnit.Framework;
using Plugins.Saneject.Editor.Menus.Toolbar;
using Tests.Saneject.Fixtures.Scripts;
using UnityEditor;
using UnityEditor.SceneManagement;
using Object = UnityEngine.Object;

namespace Tests.Saneject.Editor.Editor.Menus
{
    public class InjectMainToolbarButtonTests
    {
        [TearDown]
        public void TearDown()
        {
            Selection.objects = Array.Empty<Object>();

            TestPrefabAsset.CloseStage();
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        }

        [Test]
        public void Get_GivenSceneWithoutSelection_EnablesOnlyInjectSceneButton()
        {
            TestScene.Create(roots: 1, width: 1, depth: 1);

            InjectToolbarState toolbarState = InjectToolbarState.Get();

            Assert.That(toolbarState.InjectSceneButtonEnabled, Is.True);
            Assert.That(toolbarState.InjectHierarchiesButtonEnabled, Is.False);
            Assert.That(toolbarState.InjectPrefabButtonEnabled, Is.False);
            Assert.That(toolbarState.BatchInjectButtonEnabled, Is.False);
        }

        [Test]
        public void Get_GivenSceneSelection_EnablesSceneAndHierarchiesButtons()
        {
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            Selection.objects = new Object[] { scene.GetTransform("Root 1").gameObject };

            InjectToolbarState toolbarState = InjectToolbarState.Get();

            Assert.That(toolbarState.InjectSceneButtonEnabled, Is.True);
            Assert.That(toolbarState.InjectHierarchiesButtonEnabled, Is.True);
            Assert.That(toolbarState.InjectPrefabButtonEnabled, Is.False);
            Assert.That(toolbarState.BatchInjectButtonEnabled, Is.False);
        }

        [Test]
        public void Get_GivenPrefabStage_EnablesOnlyInjectPrefabButton()
        {
            TestPrefabAsset prefab = TestPrefabAsset.Create("Prefab Root", width: 1, depth: 1);
            TestPrefabInstance prefabStage = prefab.OpenStage();

            try
            {
                Selection.objects = Array.Empty<Object>();
                InjectToolbarState toolbarState = InjectToolbarState.Get();

                Assert.That(prefabStage, Is.Not.Null);
                Assert.That(toolbarState.InjectSceneButtonEnabled, Is.False);
                Assert.That(toolbarState.InjectHierarchiesButtonEnabled, Is.False);
                Assert.That(toolbarState.InjectPrefabButtonEnabled, Is.True);
                Assert.That(toolbarState.BatchInjectButtonEnabled, Is.False);
            }
            finally
            {
                TestPrefabAsset.CloseStage();
                prefab.Destroy();
                prefab.DeleteAsset();
            }
        }

        [Test]
        public void Get_GivenBatchAssetSelection_EnablesSceneAndBatchInjectButtons()
        {
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            string scenePath = scene.SaveToDisk();
            SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);

            try
            {
                Selection.objects = new Object[] { sceneAsset };
                InjectToolbarState toolbarState = InjectToolbarState.Get();

                Assert.That(sceneAsset, Is.Not.Null);
                Assert.That(toolbarState.InjectSceneButtonEnabled, Is.True);
                Assert.That(toolbarState.InjectHierarchiesButtonEnabled, Is.False);
                Assert.That(toolbarState.InjectPrefabButtonEnabled, Is.False);
                Assert.That(toolbarState.BatchInjectButtonEnabled, Is.True);
            }
            finally
            {
                EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                scene.DeleteFromDisk();
            }
        }
    }
}