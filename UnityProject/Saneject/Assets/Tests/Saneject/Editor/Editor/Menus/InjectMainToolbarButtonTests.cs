using System;
using System.Reflection;
using NUnit.Framework;
using Plugins.Saneject.Editor.Menus.InjectToolbar;
using Tests.Saneject.Fixtures.Scripts;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
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
        public void GetToolbarState_GivenSceneSelection_ReturnsSceneModeAndSelectionCount()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            MethodInfo getToolbarStateMethod = typeof(InjectMainToolbarButton).GetMethod
            (
                "GetToolbarState",
                BindingFlags.NonPublic | BindingFlags.Static
            );

            // Select object
            Selection.objects = new Object[] { scene.GetTransform("Root 1").gameObject };

            // Get toolbar state
            object toolbarState = getToolbarStateMethod.Invoke(null, null);
            Type toolbarStateType = toolbarState.GetType();
            PropertyInfo modeProperty = toolbarStateType.GetProperty("Mode", BindingFlags.Public | BindingFlags.Instance);
            PropertyInfo sceneObjectSelectionCountProperty = toolbarStateType.GetProperty("SceneObjectSelectionCount", BindingFlags.Public | BindingFlags.Instance);
            PropertyInfo hasBatchInjectAssetSelectionProperty = toolbarStateType.GetProperty("HasBatchInjectAssetSelection", BindingFlags.Public | BindingFlags.Instance);

            // Assert
            Assert.That(getToolbarStateMethod, Is.Not.Null);
            Assert.That(toolbarState, Is.Not.Null);
            Assert.That(modeProperty, Is.Not.Null);
            Assert.That(sceneObjectSelectionCountProperty, Is.Not.Null);
            Assert.That(hasBatchInjectAssetSelectionProperty, Is.Not.Null);
            Assert.That(modeProperty.GetValue(toolbarState)?.ToString(), Is.EqualTo("Scene"));
            Assert.That((int)sceneObjectSelectionCountProperty.GetValue(toolbarState), Is.EqualTo(1));
            Assert.That((bool)hasBatchInjectAssetSelectionProperty.GetValue(toolbarState), Is.False);
        }

        [Test]
        public void GetToolbarState_GivenPrefabStage_ReturnsPrefabMode()
        {
            // Set up prefab asset
            TestPrefabAsset prefab = TestPrefabAsset.Create("Prefab Root", width: 1, depth: 1);
            TestPrefabInstance prefabStage = prefab.OpenStage();
            MethodInfo getToolbarStateMethod = typeof(InjectMainToolbarButton).GetMethod
            (
                "GetToolbarState",
                BindingFlags.NonPublic | BindingFlags.Static
            );

            try
            {
                // Get toolbar state
                Selection.objects = Array.Empty<Object>();
                object toolbarState = getToolbarStateMethod.Invoke(null, null);
                Type toolbarStateType = toolbarState.GetType();
                PropertyInfo modeProperty = toolbarStateType.GetProperty("Mode", BindingFlags.Public | BindingFlags.Instance);
                PropertyInfo sceneObjectSelectionCountProperty = toolbarStateType.GetProperty("SceneObjectSelectionCount", BindingFlags.Public | BindingFlags.Instance);
                PropertyInfo hasBatchInjectAssetSelectionProperty = toolbarStateType.GetProperty("HasBatchInjectAssetSelection", BindingFlags.Public | BindingFlags.Instance);

                // Assert
                Assert.That(prefabStage, Is.Not.Null);
                Assert.That(getToolbarStateMethod, Is.Not.Null);
                Assert.That(toolbarState, Is.Not.Null);
                Assert.That(modeProperty, Is.Not.Null);
                Assert.That(sceneObjectSelectionCountProperty, Is.Not.Null);
                Assert.That(hasBatchInjectAssetSelectionProperty, Is.Not.Null);
                Assert.That(modeProperty.GetValue(toolbarState)?.ToString(), Is.EqualTo("Prefab"));
                Assert.That((int)sceneObjectSelectionCountProperty.GetValue(toolbarState), Is.EqualTo(0));
                Assert.That((bool)hasBatchInjectAssetSelectionProperty.GetValue(toolbarState), Is.False);
            }
            finally
            {
                TestPrefabAsset.CloseStage();
                prefab.Destroy();
                prefab.DeleteAsset();
            }
        }

        [Test]
        public void GetToolbarState_GivenBatchAssetSelection_ReturnsSceneModeAndBatchSelection()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            string scenePath = scene.SaveToDisk();
            SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
            MethodInfo getToolbarStateMethod = typeof(InjectMainToolbarButton).GetMethod
            (
                "GetToolbarState",
                BindingFlags.NonPublic | BindingFlags.Static
            );

            try
            {
                // Get toolbar state
                Selection.objects = new Object[] { sceneAsset };
                object toolbarState = getToolbarStateMethod.Invoke(null, null);
                Type toolbarStateType = toolbarState.GetType();
                PropertyInfo modeProperty = toolbarStateType.GetProperty("Mode", BindingFlags.Public | BindingFlags.Instance);
                PropertyInfo sceneObjectSelectionCountProperty = toolbarStateType.GetProperty("SceneObjectSelectionCount", BindingFlags.Public | BindingFlags.Instance);
                PropertyInfo hasBatchInjectAssetSelectionProperty = toolbarStateType.GetProperty("HasBatchInjectAssetSelection", BindingFlags.Public | BindingFlags.Instance);

                // Assert
                Assert.That(sceneAsset, Is.Not.Null);
                Assert.That(getToolbarStateMethod, Is.Not.Null);
                Assert.That(toolbarState, Is.Not.Null);
                Assert.That(modeProperty, Is.Not.Null);
                Assert.That(sceneObjectSelectionCountProperty, Is.Not.Null);
                Assert.That(hasBatchInjectAssetSelectionProperty, Is.Not.Null);
                Assert.That(modeProperty.GetValue(toolbarState)?.ToString(), Is.EqualTo("Scene"));
                Assert.That((int)sceneObjectSelectionCountProperty.GetValue(toolbarState), Is.EqualTo(0));
                Assert.That((bool)hasBatchInjectAssetSelectionProperty.GetValue(toolbarState), Is.True);
            }
            finally
            {
                EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                scene.DeleteFromDisk();
            }
        }

        [Test]
        public void ApplyToolbarState_GivenSceneModeWithSelectionAndBatchSelection_SetsExpectedDisplays()
        {
            // Find fields and methods
            FieldInfo containerField = typeof(InjectMainToolbarButton).GetField("container", BindingFlags.NonPublic | BindingFlags.Static);
            FieldInfo injectSceneButtonField = typeof(InjectMainToolbarButton).GetField("injectSceneButton", BindingFlags.NonPublic | BindingFlags.Static);
            FieldInfo injectSelectionButtonField = typeof(InjectMainToolbarButton).GetField("injectSelectionButton", BindingFlags.NonPublic | BindingFlags.Static);
            FieldInfo injectPrefabButtonField = typeof(InjectMainToolbarButton).GetField("injectPrefabButton", BindingFlags.NonPublic | BindingFlags.Static);
            FieldInfo batchInjectSelectedAssetsButtonField = typeof(InjectMainToolbarButton).GetField("batchInjectSelectedAssetsButton", BindingFlags.NonPublic | BindingFlags.Static);
            MethodInfo applyToolbarStateMethod = typeof(InjectMainToolbarButton).GetMethod("ApplyToolbarState", BindingFlags.NonPublic | BindingFlags.Static);
            Type toolbarModeType = typeof(InjectMainToolbarButton).GetNestedType("ToolbarMode", BindingFlags.NonPublic);
            Type toolbarStateType = typeof(InjectMainToolbarButton).GetNestedType("ToolbarState", BindingFlags.NonPublic);
            ConstructorInfo toolbarStateConstructor = toolbarStateType?.GetConstructor
            (
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                null,
                new[] { toolbarModeType, typeof(int), typeof(bool) },
                null
            );

            Assert.That(containerField, Is.Not.Null);
            Assert.That(injectSceneButtonField, Is.Not.Null);
            Assert.That(injectSelectionButtonField, Is.Not.Null);
            Assert.That(injectPrefabButtonField, Is.Not.Null);
            Assert.That(batchInjectSelectedAssetsButtonField, Is.Not.Null);
            Assert.That(applyToolbarStateMethod, Is.Not.Null);
            Assert.That(toolbarModeType, Is.Not.Null);
            Assert.That(toolbarStateType, Is.Not.Null);
            Assert.That(toolbarStateConstructor, Is.Not.Null);

            // Capture previous toolbar controls
            object previousContainer = containerField.GetValue(null);
            object previousInjectSceneButton = injectSceneButtonField.GetValue(null);
            object previousInjectSelectionButton = injectSelectionButtonField.GetValue(null);
            object previousInjectPrefabButton = injectPrefabButtonField.GetValue(null);
            object previousBatchInjectSelectedAssetsButton = batchInjectSelectedAssetsButtonField.GetValue(null);
            VisualElement container = new();
            ToolbarButton injectSceneButton = new();
            ToolbarButton injectSelectionButton = new();
            ToolbarButton injectPrefabButton = new();
            ToolbarButton batchInjectSelectedAssetsButton = new();

            try
            {
                // Apply toolbar state
                containerField.SetValue(null, container);
                injectSceneButtonField.SetValue(null, injectSceneButton);
                injectSelectionButtonField.SetValue(null, injectSelectionButton);
                injectPrefabButtonField.SetValue(null, injectPrefabButton);
                batchInjectSelectedAssetsButtonField.SetValue(null, batchInjectSelectedAssetsButton);

                object toolbarState = toolbarStateConstructor.Invoke(new object[]
                {
                    Enum.Parse(toolbarModeType, "Scene"),
                    1,
                    true
                });

                applyToolbarStateMethod.Invoke(null, new[] { toolbarState });

                // Assert
                Assert.That(container.style.display.value, Is.EqualTo(DisplayStyle.Flex));
                Assert.That(injectSceneButton.style.display.value, Is.EqualTo(DisplayStyle.Flex));
                Assert.That(injectSelectionButton.style.display.value, Is.EqualTo(DisplayStyle.Flex));
                Assert.That(injectPrefabButton.style.display.value, Is.EqualTo(DisplayStyle.None));
                Assert.That(batchInjectSelectedAssetsButton.style.display.value, Is.EqualTo(DisplayStyle.Flex));
            }
            finally
            {
                containerField.SetValue(null, previousContainer);
                injectSceneButtonField.SetValue(null, previousInjectSceneButton);
                injectSelectionButtonField.SetValue(null, previousInjectSelectionButton);
                injectPrefabButtonField.SetValue(null, previousInjectPrefabButton);
                batchInjectSelectedAssetsButtonField.SetValue(null, previousBatchInjectSelectedAssetsButton);
            }
        }

        [Test]
        public void ApplyToolbarState_GivenPrefabMode_SetsExpectedDisplays()
        {
            // Find fields and methods
            FieldInfo containerField = typeof(InjectMainToolbarButton).GetField("container", BindingFlags.NonPublic | BindingFlags.Static);
            FieldInfo injectSceneButtonField = typeof(InjectMainToolbarButton).GetField("injectSceneButton", BindingFlags.NonPublic | BindingFlags.Static);
            FieldInfo injectSelectionButtonField = typeof(InjectMainToolbarButton).GetField("injectSelectionButton", BindingFlags.NonPublic | BindingFlags.Static);
            FieldInfo injectPrefabButtonField = typeof(InjectMainToolbarButton).GetField("injectPrefabButton", BindingFlags.NonPublic | BindingFlags.Static);
            FieldInfo batchInjectSelectedAssetsButtonField = typeof(InjectMainToolbarButton).GetField("batchInjectSelectedAssetsButton", BindingFlags.NonPublic | BindingFlags.Static);
            MethodInfo applyToolbarStateMethod = typeof(InjectMainToolbarButton).GetMethod("ApplyToolbarState", BindingFlags.NonPublic | BindingFlags.Static);
            Type toolbarModeType = typeof(InjectMainToolbarButton).GetNestedType("ToolbarMode", BindingFlags.NonPublic);
            Type toolbarStateType = typeof(InjectMainToolbarButton).GetNestedType("ToolbarState", BindingFlags.NonPublic);
            ConstructorInfo toolbarStateConstructor = toolbarStateType?.GetConstructor
            (
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                null,
                new[] { toolbarModeType, typeof(int), typeof(bool) },
                null
            );

            Assert.That(containerField, Is.Not.Null);
            Assert.That(injectSceneButtonField, Is.Not.Null);
            Assert.That(injectSelectionButtonField, Is.Not.Null);
            Assert.That(injectPrefabButtonField, Is.Not.Null);
            Assert.That(batchInjectSelectedAssetsButtonField, Is.Not.Null);
            Assert.That(applyToolbarStateMethod, Is.Not.Null);
            Assert.That(toolbarModeType, Is.Not.Null);
            Assert.That(toolbarStateType, Is.Not.Null);
            Assert.That(toolbarStateConstructor, Is.Not.Null);

            // Capture previous toolbar controls
            object previousContainer = containerField.GetValue(null);
            object previousInjectSceneButton = injectSceneButtonField.GetValue(null);
            object previousInjectSelectionButton = injectSelectionButtonField.GetValue(null);
            object previousInjectPrefabButton = injectPrefabButtonField.GetValue(null);
            object previousBatchInjectSelectedAssetsButton = batchInjectSelectedAssetsButtonField.GetValue(null);
            VisualElement container = new();
            ToolbarButton injectSceneButton = new();
            ToolbarButton injectSelectionButton = new();
            ToolbarButton injectPrefabButton = new();
            ToolbarButton batchInjectSelectedAssetsButton = new();

            try
            {
                // Apply toolbar state
                containerField.SetValue(null, container);
                injectSceneButtonField.SetValue(null, injectSceneButton);
                injectSelectionButtonField.SetValue(null, injectSelectionButton);
                injectPrefabButtonField.SetValue(null, injectPrefabButton);
                batchInjectSelectedAssetsButtonField.SetValue(null, batchInjectSelectedAssetsButton);

                object toolbarState = toolbarStateConstructor.Invoke(new object[]
                {
                    Enum.Parse(toolbarModeType, "Prefab"),
                    0,
                    false
                });

                applyToolbarStateMethod.Invoke(null, new[] { toolbarState });

                // Assert
                Assert.That(container.style.display.value, Is.EqualTo(DisplayStyle.Flex));
                Assert.That(injectSceneButton.style.display.value, Is.EqualTo(DisplayStyle.None));
                Assert.That(injectSelectionButton.style.display.value, Is.EqualTo(DisplayStyle.None));
                Assert.That(injectPrefabButton.style.display.value, Is.EqualTo(DisplayStyle.Flex));
                Assert.That(batchInjectSelectedAssetsButton.style.display.value, Is.EqualTo(DisplayStyle.None));
            }
            finally
            {
                containerField.SetValue(null, previousContainer);
                injectSceneButtonField.SetValue(null, previousInjectSceneButton);
                injectSelectionButtonField.SetValue(null, previousInjectSelectionButton);
                injectPrefabButtonField.SetValue(null, previousInjectPrefabButton);
                batchInjectSelectedAssetsButtonField.SetValue(null, previousBatchInjectSelectedAssetsButton);
            }
        }

        [Test]
        public void ApplyToolbarState_GivenSceneModeWithoutSelection_SetsExpectedDisplays()
        {
            // Find fields and methods
            FieldInfo containerField = typeof(InjectMainToolbarButton).GetField("container", BindingFlags.NonPublic | BindingFlags.Static);
            FieldInfo injectSceneButtonField = typeof(InjectMainToolbarButton).GetField("injectSceneButton", BindingFlags.NonPublic | BindingFlags.Static);
            FieldInfo injectSelectionButtonField = typeof(InjectMainToolbarButton).GetField("injectSelectionButton", BindingFlags.NonPublic | BindingFlags.Static);
            FieldInfo injectPrefabButtonField = typeof(InjectMainToolbarButton).GetField("injectPrefabButton", BindingFlags.NonPublic | BindingFlags.Static);
            FieldInfo batchInjectSelectedAssetsButtonField = typeof(InjectMainToolbarButton).GetField("batchInjectSelectedAssetsButton", BindingFlags.NonPublic | BindingFlags.Static);
            MethodInfo applyToolbarStateMethod = typeof(InjectMainToolbarButton).GetMethod("ApplyToolbarState", BindingFlags.NonPublic | BindingFlags.Static);
            Type toolbarModeType = typeof(InjectMainToolbarButton).GetNestedType("ToolbarMode", BindingFlags.NonPublic);
            Type toolbarStateType = typeof(InjectMainToolbarButton).GetNestedType("ToolbarState", BindingFlags.NonPublic);
            ConstructorInfo toolbarStateConstructor = toolbarStateType?.GetConstructor
            (
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                null,
                new[] { toolbarModeType, typeof(int), typeof(bool) },
                null
            );

            Assert.That(containerField, Is.Not.Null);
            Assert.That(injectSceneButtonField, Is.Not.Null);
            Assert.That(injectSelectionButtonField, Is.Not.Null);
            Assert.That(injectPrefabButtonField, Is.Not.Null);
            Assert.That(batchInjectSelectedAssetsButtonField, Is.Not.Null);
            Assert.That(applyToolbarStateMethod, Is.Not.Null);
            Assert.That(toolbarModeType, Is.Not.Null);
            Assert.That(toolbarStateType, Is.Not.Null);
            Assert.That(toolbarStateConstructor, Is.Not.Null);

            // Capture previous toolbar controls
            object previousContainer = containerField.GetValue(null);
            object previousInjectSceneButton = injectSceneButtonField.GetValue(null);
            object previousInjectSelectionButton = injectSelectionButtonField.GetValue(null);
            object previousInjectPrefabButton = injectPrefabButtonField.GetValue(null);
            object previousBatchInjectSelectedAssetsButton = batchInjectSelectedAssetsButtonField.GetValue(null);
            VisualElement container = new();
            ToolbarButton injectSceneButton = new();
            ToolbarButton injectSelectionButton = new();
            ToolbarButton injectPrefabButton = new();
            ToolbarButton batchInjectSelectedAssetsButton = new();

            try
            {
                // Apply toolbar state
                containerField.SetValue(null, container);
                injectSceneButtonField.SetValue(null, injectSceneButton);
                injectSelectionButtonField.SetValue(null, injectSelectionButton);
                injectPrefabButtonField.SetValue(null, injectPrefabButton);
                batchInjectSelectedAssetsButtonField.SetValue(null, batchInjectSelectedAssetsButton);

                object toolbarState = toolbarStateConstructor.Invoke(new object[]
                {
                    Enum.Parse(toolbarModeType, "Scene"),
                    0,
                    false
                });

                applyToolbarStateMethod.Invoke(null, new[] { toolbarState });

                // Assert
                Assert.That(container.style.display.value, Is.EqualTo(DisplayStyle.Flex));
                Assert.That(injectSceneButton.style.display.value, Is.EqualTo(DisplayStyle.Flex));
                Assert.That(injectSelectionButton.style.display.value, Is.EqualTo(DisplayStyle.None));
                Assert.That(injectPrefabButton.style.display.value, Is.EqualTo(DisplayStyle.None));
                Assert.That(batchInjectSelectedAssetsButton.style.display.value, Is.EqualTo(DisplayStyle.None));
            }
            finally
            {
                containerField.SetValue(null, previousContainer);
                injectSceneButtonField.SetValue(null, previousInjectSceneButton);
                injectSelectionButtonField.SetValue(null, previousInjectSelectionButton);
                injectPrefabButtonField.SetValue(null, previousInjectPrefabButton);
                batchInjectSelectedAssetsButtonField.SetValue(null, previousBatchInjectSelectedAssetsButton);
            }
        }

        [Test]
        public void ApplyToolbarState_GivenNoModeAndNoBatchSelection_HidesAllControls()
        {
            // Find fields and methods
            FieldInfo containerField = typeof(InjectMainToolbarButton).GetField("container", BindingFlags.NonPublic | BindingFlags.Static);
            FieldInfo injectSceneButtonField = typeof(InjectMainToolbarButton).GetField("injectSceneButton", BindingFlags.NonPublic | BindingFlags.Static);
            FieldInfo injectSelectionButtonField = typeof(InjectMainToolbarButton).GetField("injectSelectionButton", BindingFlags.NonPublic | BindingFlags.Static);
            FieldInfo injectPrefabButtonField = typeof(InjectMainToolbarButton).GetField("injectPrefabButton", BindingFlags.NonPublic | BindingFlags.Static);
            FieldInfo batchInjectSelectedAssetsButtonField = typeof(InjectMainToolbarButton).GetField("batchInjectSelectedAssetsButton", BindingFlags.NonPublic | BindingFlags.Static);
            MethodInfo applyToolbarStateMethod = typeof(InjectMainToolbarButton).GetMethod("ApplyToolbarState", BindingFlags.NonPublic | BindingFlags.Static);
            Type toolbarModeType = typeof(InjectMainToolbarButton).GetNestedType("ToolbarMode", BindingFlags.NonPublic);
            Type toolbarStateType = typeof(InjectMainToolbarButton).GetNestedType("ToolbarState", BindingFlags.NonPublic);
            ConstructorInfo toolbarStateConstructor = toolbarStateType?.GetConstructor
            (
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                null,
                new[] { toolbarModeType, typeof(int), typeof(bool) },
                null
            );

            Assert.That(containerField, Is.Not.Null);
            Assert.That(injectSceneButtonField, Is.Not.Null);
            Assert.That(injectSelectionButtonField, Is.Not.Null);
            Assert.That(injectPrefabButtonField, Is.Not.Null);
            Assert.That(batchInjectSelectedAssetsButtonField, Is.Not.Null);
            Assert.That(applyToolbarStateMethod, Is.Not.Null);
            Assert.That(toolbarModeType, Is.Not.Null);
            Assert.That(toolbarStateType, Is.Not.Null);
            Assert.That(toolbarStateConstructor, Is.Not.Null);

            // Capture previous toolbar controls
            object previousContainer = containerField.GetValue(null);
            object previousInjectSceneButton = injectSceneButtonField.GetValue(null);
            object previousInjectSelectionButton = injectSelectionButtonField.GetValue(null);
            object previousInjectPrefabButton = injectPrefabButtonField.GetValue(null);
            object previousBatchInjectSelectedAssetsButton = batchInjectSelectedAssetsButtonField.GetValue(null);
            VisualElement container = new();
            ToolbarButton injectSceneButton = new();
            ToolbarButton injectSelectionButton = new();
            ToolbarButton injectPrefabButton = new();
            ToolbarButton batchInjectSelectedAssetsButton = new();

            try
            {
                // Apply toolbar state
                containerField.SetValue(null, container);
                injectSceneButtonField.SetValue(null, injectSceneButton);
                injectSelectionButtonField.SetValue(null, injectSelectionButton);
                injectPrefabButtonField.SetValue(null, injectPrefabButton);
                batchInjectSelectedAssetsButtonField.SetValue(null, batchInjectSelectedAssetsButton);

                object toolbarState = toolbarStateConstructor.Invoke(new object[]
                {
                    Enum.Parse(toolbarModeType, "None"),
                    0,
                    false
                });

                applyToolbarStateMethod.Invoke(null, new[] { toolbarState });

                // Assert
                Assert.That(container.style.display.value, Is.EqualTo(DisplayStyle.None));
                Assert.That(injectSceneButton.style.display.value, Is.EqualTo(DisplayStyle.None));
                Assert.That(injectSelectionButton.style.display.value, Is.EqualTo(DisplayStyle.None));
                Assert.That(injectPrefabButton.style.display.value, Is.EqualTo(DisplayStyle.None));
                Assert.That(batchInjectSelectedAssetsButton.style.display.value, Is.EqualTo(DisplayStyle.None));
            }
            finally
            {
                containerField.SetValue(null, previousContainer);
                injectSceneButtonField.SetValue(null, previousInjectSceneButton);
                injectSelectionButtonField.SetValue(null, previousInjectSelectionButton);
                injectPrefabButtonField.SetValue(null, previousInjectPrefabButton);
                batchInjectSelectedAssetsButtonField.SetValue(null, previousBatchInjectSelectedAssetsButton);
            }
        }
    }
}
