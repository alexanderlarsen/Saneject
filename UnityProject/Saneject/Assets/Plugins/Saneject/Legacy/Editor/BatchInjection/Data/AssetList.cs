using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Legacy.Editor.BatchInjection.Enums;
using Plugins.Saneject.Legacy.Editor.BatchInjection.Utilities;
using UnityEditor;
using UnityEngine;

namespace Plugins.Saneject.Legacy.Editor.BatchInjection.Data
{
    [Serializable]
    public class AssetList
    {
        [SerializeField]
        private SortMode sortMode = SortMode.NameAtoZ;

        [SerializeField]
        private List<AssetData> list = new();

        [SerializeField]
        private Vector2 scroll;

        public int EnabledCount => list.Count(item => item.Enabled);
        public int TotalCount => list.Count;
        public IList Elements => list;

        public SortMode SortMode
        {
            get => sortMode;
            set => sortMode = value;
        }

        public Vector2 Scroll
        {
            get => scroll;
            set => scroll = value;
        }

        public bool TryAddByPath(string path)
        {
            string guid = AssetDatabase.AssetPathToGUID(path);
            return TryAddByGuid(guid);
        }

        public bool TryAddByGuid(string guid)
        {
            if (list.Any(item => item.Guid == guid))
                return false;

            list.Add(new AssetData(guid));
            return true;
        }

        public AssetData GetElementAt(int index)
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
            SortingUtils.SortList(list, sortMode);
        }

        public AssetData[] GetEnabled()
        {
            return list
                .Where(item => item.Enabled)
                .ToArray();
        }

        public IEnumerable<string> FindGuidsNotInList(IEnumerable<string> guids)
        {
            return guids.Where(guid => list.All(item => item.Guid != guid));
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
    }
}