using System;
using Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjection.Enums;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjection.Data
{
    [Serializable]
    public class AssetData
    {
        [SerializeField]
        private string guid;

        [SerializeField]
        private bool enabled = true;

        [SerializeField]
        private InjectionStatus status;

        [NonSerialized]
        private Object asset;

        public AssetData(string guid)
        {
            this.guid = guid;
        }

        public string Guid => guid;
        public Object Asset => asset ??= AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(guid));
        public string Path => AssetDatabase.GUIDToAssetPath(guid);
        public string Name => Asset ? Asset.name : string.Empty;

        public bool Enabled
        {
            get => enabled;
            set => enabled = value;
        }

        public InjectionStatus Status
        {
            get => status;
            set => status = value;
        }
    }
}