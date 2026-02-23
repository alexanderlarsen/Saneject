using System;
using Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Enums;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Data
{
    [Serializable]
    public class BatchInjectorData
    {
        [SerializeField]
        private AssetList sceneList = new();

        [SerializeField]
        private AssetList prefabList = new();

        [SerializeField]
        private WindowTab windowTab = WindowTab.Scenes;

        private bool isDirty;

        public AssetList SceneList => sceneList;
        public AssetList PrefabList => prefabList;

        public WindowTab WindowTab
        {
            get => windowTab;
            set => windowTab = value;
        }

        public bool IsDirty
        {
            get => isDirty;
            set => isDirty = value;
        }
    }
}