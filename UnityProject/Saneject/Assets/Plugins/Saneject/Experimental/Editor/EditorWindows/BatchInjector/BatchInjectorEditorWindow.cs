using Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Controls;
using Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Data;
using Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Drawers;
using Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Persistence;
using Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Utilities;
using UnityEditor;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector
{
    public class BatchInjectorEditorWindow : EditorWindow
    {
        private const float WindowPadding = 5f;

        private BatchInjectorData batchInjectorData = new();
        private ReorderableAssetList sceneList;
        private ReorderableAssetList prefabList;
        private Rect sceneListRect;
        private Rect prefabListRect;
        private bool clickedAnyListItem;
        private GUIStyle titleStyle;

        private void OnEnable()
        {
            batchInjectorData = Storage.LoadOrCreateData();

            sceneList = new ReorderableAssetList
            (
                assetList: batchInjectorData.sceneList,
                onModified: () => batchInjectorData.isDirty = true
            );

            prefabList = new ReorderableAssetList
            (
                assetList: batchInjectorData.prefabList,
                onModified: () => batchInjectorData.isDirty = true
            );
        }

        private void OnDisable()
        {
            Storage.SaveIfDirty(batchInjectorData);
        }

        [MenuItem("Saneject/Batch Inject/Open Batch Injector Window")]
        public static void ShowWindow()
        {
            BatchInjectorEditorWindow window = GetWindow<BatchInjectorEditorWindow>("Saneject Batch Injector");
            window.minSize = new Vector2(420, 400);
            window.Show();
        }

        private void OnGUI()
        {
            DragAndDropUtility.HandleDragAndDrop
            (
                dropArea: position,
                batchInjectorData: batchInjectorData,
                sceneList: sceneList,
                prefabList: prefabList,
                repaint: Repaint
            );

            GUILayout.BeginArea
            (
                new Rect
                (
                    x: WindowPadding,
                    y: WindowPadding,
                    width: position.width - WindowPadding * 2,
                    height: position.height - WindowPadding * 2
                )
            );

            DrawHeaderLabels();

            WindowTabsDrawer.DrawTabs
            (
                batchInjectorData,
                sceneList,
                sceneListRect,
                prefabList,
                prefabListRect,
                ref clickedAnyListItem,
                Repaint
            );

            GUILayout.FlexibleSpace();
            InjectButtonsDrawer.DrawInjectButtons(batchInjectorData);

            InputUtility.HandleInput
            (
                clickedAnyListItem: ref clickedAnyListItem,
                tab: batchInjectorData.windowTab,
                sceneList: sceneList,
                prefabList: prefabList,
                repaint: Repaint
            );

            GUILayout.EndArea();
            Storage.SaveIfDirty(batchInjectorData);
        }

        private void DrawHeaderLabels()
        {
            titleStyle ??= new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                normal =
                {
                    textColor = EditorStyles.label.normal.textColor
                }
            };

            EditorGUILayout.LabelField("Batch Injector", titleStyle);
            EditorGUILayout.LabelField("Drag and drop scenes and prefabs anywhere in the window to add them to each list. Then click one of the Inject-buttons to inject all selected scenes and/or prefabs in one go.", EditorStyles.wordWrappedLabel);
            EditorGUILayout.LabelField("The status icons show the result of the last injection run from this window.", EditorStyles.wordWrappedLabel);
            GUILayout.Space(8);
        }
    }
}