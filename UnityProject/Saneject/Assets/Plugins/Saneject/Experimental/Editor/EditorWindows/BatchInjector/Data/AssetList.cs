using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Enums;
using Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Utilities;
using UnityEditor;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Data
{
    [Serializable]
    public class AssetList
    {
        #region Fields/properties

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

        #endregion

        #region List CRUD methods

        public bool TryAddAssetByPath(string path)
        {
            string guid = AssetDatabase.AssetPathToGUID(path);
            return TryAddAssetByGuid(guid);
        }

        public bool TryAddAssetByGuid(string guid)
        {
            if (list.Any(item => item.Guid == guid))
                return false;

            list.Add(new AssetData(guid));
            return true;
        }

        public AssetData[] GetEnabled()
        {
            return list
                .Where(item => item.Enabled)
                .ToArray();
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

        public void Clear()
        {
            list.Clear();
        }

        #endregion

        #region Helper methods

        public void Sort()
        {
            SortingUtility.SortList(list, sortMode);
        }

        public void TrySortByEnabledOrDisabled()
        {
            if (sortMode is not (SortMode.EnabledToDisabled or SortMode.DisabledToEnabled))
                return;

            Sort();
        }

        public int FindIndexByPath(string path)
        {
            return list.FindIndex(asset => asset.GetAssetPath() == path);
        }

        public IEnumerable<string> FindGuidsNotInList(IEnumerable<string> guids)
        {
            return guids.Where(guid => list.All(item => item.Guid != guid));
        }

        #endregion
    }
}