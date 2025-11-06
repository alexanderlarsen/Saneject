using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Plugins.Saneject.Editor.EditorWindows.BatchInjection
{
    public static class AssetListExtensions
    {
        public static bool TryAdd(
            this List<AssetData> list,
            string assetPath)
        {
            if (list.Any(item => item.Path == assetPath))
                return false;

            list.Add(new AssetData(assetPath));
            return true;
        }

        public static void Sort(
            this List<AssetData> list,
            SortMode sortMode)
        {
            AssetListSorter.SortList(list, sortMode);
        }

        public static AssetData[] GetEnabled(this List<AssetData> list)
        {
            return list
                .Where(item => item.Enabled)
                .ToArray();
        }

        public static List<string> GetAssetPathsNotInList(this List<string> paths, List<AssetData> list)
        {
            return paths
                .Where(path => list.All(s => s.Path != path))
                .Where(path => AssetDatabase.LoadAssetAtPath<GameObject>(path))
                .ToList();
        }
    }
}