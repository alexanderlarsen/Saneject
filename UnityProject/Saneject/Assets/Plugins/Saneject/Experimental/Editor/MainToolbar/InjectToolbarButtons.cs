using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Plugins.Saneject.Experimental.Editor.Data.Context;
using Plugins.Saneject.Experimental.Editor.Utilities;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Experimental.Editor.MainToolbar
{
    [EditorBrowsable(EditorBrowsableState.Never), InitializeOnLoad]
    public static class InjectMainToolbarButton
    {
        private const string ToolbarZoneName = "ToolbarZoneLeftAlign";
        private const string ContainerName = "Saneject-Inject-MainToolbar";

        private static readonly Type ToolbarType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.Toolbar");
        private static readonly FieldInfo RootField = ToolbarType?.GetField("m_Root", BindingFlags.Instance | BindingFlags.NonPublic);

        private static ToolbarState currentState;

        static InjectMainToolbarButton()
        {
            EditorApplication.update += TryAttachToToolbar;
        }

        private enum ToolbarMode
        {
            None,
            Scene,
            Prefab
        }

        private static void TryAttachToToolbar()
        {
            if (ToolbarType == null || RootField == null)
                return;

            ScriptableObject toolbar = GetToolbarInstance();

            if (toolbar == null)
                return;

            VisualElement toolbarRoot = RootField.GetValue(toolbar) as VisualElement;
            VisualElement toolbarZone = toolbarRoot?.Q(ToolbarZoneName);

            if (toolbarZone == null)
                return;

            ToolbarState desiredState = GetToolbarState();
            VisualElement existingContainer = toolbarZone.Q(ContainerName);

            if (desiredState.Mode == ToolbarMode.None)
            {
                existingContainer?.RemoveFromHierarchy();
                currentState = desiredState;
                return;
            }

            if (existingContainer == null)
            {
                toolbarZone.Add(CreateButtonContainer(desiredState));
                currentState = desiredState;
                return;
            }

            if (currentState.Equals(desiredState))
                return;

            existingContainer.RemoveFromHierarchy();
            toolbarZone.Add(CreateButtonContainer(desiredState));
            currentState = desiredState;
        }

        private static ToolbarState GetToolbarState()
        {
            ToolbarMode mode = PrefabStageUtility.GetCurrentPrefabStage() != null
                ? ToolbarMode.Prefab
                : SceneManager.sceneCount > 0
                    ? ToolbarMode.Scene
                    : ToolbarMode.None;

            return new ToolbarState
            (
                mode: mode,
                sceneObjectSelectionCount: Selection.gameObjects.Count(go => go.scene.IsValid())
            );
        }

        private static ScriptableObject GetToolbarInstance()
        {
            Object[] toolbars = Resources.FindObjectsOfTypeAll(ToolbarType);
            return toolbars.Length > 0 ? toolbars[0] as ScriptableObject : null;
        }

        private static VisualElement CreateButtonContainer(ToolbarState toolbarState)
        {
            VisualElement container = new()
            {
                name = ContainerName
            };

            container.style.flexDirection = FlexDirection.Row;
            container.style.alignItems = Align.Center;
            container.style.flexShrink = 0;
            container.style.marginRight = 4;

            if (toolbarState.Mode == ToolbarMode.Scene)
            {
                container.Add(CreateInjectSceneDropdown());

                if (toolbarState.SceneObjectSelectionCount > 0)
                    container.Add(CreateInjectSelectionDropdown(toolbarState.SceneObjectSelectionCount));
            }
            else if (toolbarState.Mode == ToolbarMode.Prefab)
            {
                container.Add(CreateInjectPrefabDropdown());
            }

            return container;
        }

        private static ToolbarButton CreateInjectSceneDropdown()
        {
            ToolbarButton button = new()
            {
                text = "Inject Scene",
                tooltip = "Injects everything in the current scene."
            };

            button.clicked += () => InjectionUtility.InjectCurrentScene(ContextWalkFilter.AllContexts);
            return button;
        }

        private static ToolbarButton CreateInjectSelectionDropdown(int selectionCount)
        {
            ToolbarButton button = new()
            {
                text = $"Inject Scene {(selectionCount == 1 ? "Hierarchy" : "Hierarchies")}",
                tooltip = $"Inject everything in the selected scene hierarch{(selectionCount == 1 ? "y" : "ies")}."
            };

            button.clicked += () => InjectionUtility.InjectSelectedSceneHierarchies(ContextWalkFilter.AllContexts);
            return button;
        }

        private static ToolbarButton CreateInjectPrefabDropdown()
        {
            ToolbarButton button = new()
            {
                text = "Inject Prefab Asset",
                tooltip = "Injects everything in the current prefab asset."
            };

            button.clicked += () => InjectionUtility.InjectCurrentPrefabAsset(ContextWalkFilter.AllContexts);
            return button;
        }

        private readonly struct ToolbarState : IEquatable<ToolbarState>
        {
            public ToolbarState(
                ToolbarMode mode,
                int sceneObjectSelectionCount)
            {
                Mode = mode;
                SceneObjectSelectionCount = sceneObjectSelectionCount;
            }

            public ToolbarMode Mode { get; }
            public int SceneObjectSelectionCount { get; }

            public bool Equals(ToolbarState other)
            {
                return Mode == other.Mode &&
                       SceneObjectSelectionCount == other.SceneObjectSelectionCount;
            }
        }
    }
}