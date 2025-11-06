using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Plugins.Saneject.Editor.EditorWindows.BatchInjection
{
    [Serializable]
    public class AssetList
    {
        [SerializeField]
        private SortMode sortMode = SortMode.NameAtoZ;

        [SerializeField]
        private List<AssetItem> list = new();

        public int EnabledCount { get; private set; }
        public int TotalCount => list.Count;
        public IList Elements => list;
        public SortMode SortMode => sortMode;

        public bool TryAdd(string assetPath)
        {
            if (list.Any(item => item.Path == assetPath))
                return false;

            list.Add(new AssetItem(assetPath));
            return true;
        }

        public AssetItem GetElementAt(int index)
        {
            if (index < 0 || index >= list.Count)
                throw new IndexOutOfRangeException();
            
            return list[index];
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= list.Count)
                return;

            list.RemoveAt(index);
        }

        public void Sort()
        {
            AssetListSorter.SortList(list, sortMode);
        }

        public void UpdateEnabledCount()
        {
            EnabledCount = list.Count(item => item.Enabled);
        }

        public AssetItem[] GetEnabled()
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
            if (sortMode is not (SortMode.EnabledToDisabled or SortMode.DisabledToEnabled))
                return;

            Sort();
        }

        public void Clear()
        {
            list.Clear();
        }

        public int FindIndexByPath(string path)
        {
            return list.FindIndex(asset => asset.Path == path);
        }

        public void SetSortMode(SortMode sortMode)
        {
            this.sortMode = sortMode;
        }
    }
}