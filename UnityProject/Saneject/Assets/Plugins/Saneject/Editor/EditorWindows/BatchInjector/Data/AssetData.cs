using System;
using System.ComponentModel;
using Plugins.Saneject.Editor.Data.BatchInjection;
using Plugins.Saneject.Editor.Data.Context;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Editor.EditorWindows.BatchInjector.Data
{
    [EditorBrowsable(EditorBrowsableState.Never), Serializable]
    public abstract class AssetData
    {
        protected AssetData(string guid)
        {
            this.guid = guid;
        }

        #region Fields/properties

        [SerializeField]
        private string guid;

        [SerializeField]
        private bool enabled = true;

        [SerializeField]
        private InjectionStatus status;
        
        [SerializeField]
        private ContextWalkFilter walkFilter;

        private Object asset;

        public string Guid => guid;

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
        
        public ContextWalkFilter ContextWalkFilter
        {
            get => walkFilter;
            set => walkFilter = value;
        }

        #endregion

        #region Asset methods

        public Object GetAsset()
        {
            if (asset == null)
            {
                string path = GetAssetPath();

                if (string.IsNullOrEmpty(path))
                    return null;

                asset = AssetDatabase.LoadAssetAtPath<Object>(path);
            }

            return asset;
        }

        public string GetAssetPath()
        {
            return string.IsNullOrEmpty(guid)
                ? string.Empty
                : AssetDatabase.GUIDToAssetPath(guid);
        }

        public string GetAssetName()
        {
            return GetAsset()?.name ?? string.Empty;
        }

        #endregion
    }
}