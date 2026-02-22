using System;
using Plugins.Saneject.Experimental.Editor.Data.Context;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Data
{
    [Serializable]
    public class SceneAssetData : AssetData
    {
        [SerializeField]
        private ContextWalkFilter walkFilter;

        public SceneAssetData(string guid) : base(guid)
        {
        }

        public ContextWalkFilter ContextWalkFilter
        {
            get => walkFilter;
            set => walkFilter = value;
        }
    }
} 