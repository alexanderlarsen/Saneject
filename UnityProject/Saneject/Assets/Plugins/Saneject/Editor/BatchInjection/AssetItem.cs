using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Editor.BatchInjection
{
    [Serializable]
    public class AssetItem
    {
        [SerializeField]
        private string guid;

        [SerializeField]
        private bool enabled = true;

        [NonSerialized]
        private Object asset;

        public AssetItem(string guid)
        {
            this.guid = guid;
        }

        public string Guid => guid;
        public Object Asset => asset ??= AssetDatabase.LoadAssetByGUID<Object>(new GUID(guid));
        public string Path => AssetDatabase.GUIDToAssetPath(guid);
        public string Name => Asset ? Asset.name : string.Empty;

        public bool Enabled
        {
            get => enabled;
            set => enabled = value;
        }
    }
}