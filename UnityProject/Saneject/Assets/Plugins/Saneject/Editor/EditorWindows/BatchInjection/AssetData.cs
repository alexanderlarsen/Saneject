using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Editor.EditorWindows.BatchInjection
{
    [Serializable]
    public class AssetData
    {
        [SerializeField]
        private string path;

        [SerializeField]
        private string name;

        [SerializeField]
        private bool enabled = true;

        [NonSerialized]
        private Object cachedAsset;

        public AssetData(string path)
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