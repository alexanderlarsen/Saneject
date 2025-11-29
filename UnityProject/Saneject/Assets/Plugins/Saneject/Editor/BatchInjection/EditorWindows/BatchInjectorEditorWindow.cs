using Plugins.Saneject.Editor.BatchInjection.Data;
using Plugins.Saneject.Editor.BatchInjection.Drawers;
using Plugins.Saneject.Editor.BatchInjection.Persistence;
using Plugins.Saneject.Editor.BatchInjection.Utilities;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Plugins.Saneject.Editor.BatchInjection.EditorWindows
{
    public class BatchInjectorEditorWindow : EditorWindow
    {
        private const float WindowPadding = 5f;

        private BatchInjectorData injectorData = new();
        private ReorderableList sceneList;
        private ReorderableList prefabList;
        private Rect sceneListRect;
        private Rect prefabListRect;
        private bool clickedAnyListItem;
        private GUIStyle titleStyle;

        [MenuItem("Saneject/Batch Injector")]
        public static void ShowWindow()
        {
            BatchInjectorEditorWindow window = GetWindow<BatchInjectorEditorWindow>("Saneject Batch Injector");
            window.minSize = new Vector2(420, 400);
            window.Show();
        }

        private void OnEnable()
        {
            injectorData = Storage.LoadOrCreateData();

            sceneList = ReorderableListUtils.CreateReorderableList
            (
                injectorData: injectorData,
                assetList: injectorData.sceneList,
                onModified: () => Storage.SaveData(injectorData)
            );

            prefabList = ReorderableListUtils.CreateReorderableList
            (
                injectorData: injectorData,
                assetList: injectorData.prefabList,
                onModified: () => Storage.SaveData(injectorData)
            );
        }

        private void OnDisable()
        {
            Storage.SaveData(injectorData);
        }

        private void OnGUI()
        {
            DragAndDropUtils.HandleDragAndDrop
            (
                dropArea: position,
                injectorData: injectorData,
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
                injectorData,
                sceneList,
                sceneListRect,
                prefabList,
                prefabListRect,
                ref clickedAnyListItem,
                Repaint
            );

            GUILayout.FlexibleSpace();
            InjectButtonsDrawer.DrawInjectButtons(injectorData);

            AssetListDrawer.HandleInput
            (
                clickedAnyListItem: ref clickedAnyListItem,
                tab: injectorData.windowTab,
                sceneList: sceneList,
                prefabList: prefabList,
                repaint: Repaint
            );

            GUILayout.EndArea();
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