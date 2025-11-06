using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Plugins.Saneject.Editor.EditorWindows.BatchInjection
{
    [Serializable]
    public class AssetList
    {
        public List<AssetData> list = new();
        public SortMode sortMode = SortMode.NameAtoZ;

        public int EnabledCount { get; private set; }
        
        public bool TryAdd(string assetPath)
        {
            if (list.Any(item => item.Path == assetPath))
                return false;

            list.Add(new AssetData(assetPath));
            return true;
        }

        public void Sort()
        {
            AssetListSorter.SortList(list, sortMode);
        }

        public void UpdateEnabledCount()
        {
            EnabledCount = list.Count(item => item.Enabled);
        }
        
        public AssetData[] GetEnabled()
        {
            return list
                .Where(item => item.Enabled)
                .ToArray();
        }

        public List<string> GetAssetPathsNotInList(List<string> paths)
        {
            return paths
                .Where(path => list.All(s => s.Path != path))
                .ToList();
        }

        public void TrySortByEnabledOrDisabled()
        {
            if (sortMode is SortMode.EnabledToDisabled or SortMode.DisabledToEnabled)
                Sort();
        }
        
        public void Clear()
        {
            list.Clear();
        }
    }
}