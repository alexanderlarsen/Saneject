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
        private string path;

        [SerializeField]
        private string name;

        [SerializeField]
        private bool enabled = true;

        [NonSerialized]
        private Object cachedAsset;

        public AssetItem(string path)
        {
            this.path = path;
            name = System.IO.Path.GetFileNameWithoutExtension(path);
        }

        public string Path => path;
        public string Name => name;

        public bool Enabled
        {
            get => enabled;
            set => enabled = value;
        }

        public Object Asset => cachedAsset ??= AssetDatabase.LoadAssetAtPath<Object>(Path);
    }
}