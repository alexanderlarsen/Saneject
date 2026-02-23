using System;
using System.Collections.Generic;
using Plugins.Saneject.Experimental.Editor.Data.BatchInjection;
using Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Data;
using Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Enums;

namespace Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Utilities
{
    public static class SortingUtility
    {
        public static void SortList(
            List<AssetData> list,
            SortMode mode)
        {
            if (mode == SortMode.Custom || list is not { Count: > 1 })
                return;

            switch (mode)
            {
                case SortMode.DisabledToEnabled:
                    list.Sort((a, b) =>
                    {
                        int flagCompare = a.Enabled.CompareTo(b.Enabled);

                        if (flagCompare != 0)
                            return flagCompare;

                        // Secondary alphabetical sort (by Name, then Path)
                        int nameCompare = Compare(a.GetAssetName(), b.GetAssetName());
                        
                        return nameCompare != 0 
                            ? nameCompare 
                            : Compare(a.GetAssetPath(), b.GetAssetPath());
                    });

                    return;

                case SortMode.EnabledToDisabled:
                    list.Sort((a, b) =>
                    {
                        int flagCompare = b.Enabled.CompareTo(a.Enabled);

                        if (flagCompare != 0)
                            return flagCompare;

                        // Secondary alphabetical sort (by Name, then Path)
                        int nameCompare = Compare(a.GetAssetName(), b.GetAssetName());
                        
                        return nameCompare != 0 
                            ? nameCompare 
                            : Compare(a.GetAssetPath(), b.GetAssetPath());
                    });

                    return;
                
                
                case SortMode.StatusSuccessToError:
                    list.Sort((a, b) =>
                    {
                        int statusCompare = GetStatusPrioritySuccessToError(a.Status)
                            .CompareTo(GetStatusPrioritySuccessToError(b.Status));

                        return statusCompare != 0 
                            ? statusCompare 
                            : SecondaryAlphabeticalCompare(a, b);
                    });
                    return;

                case SortMode.StatusErrorToSuccess:
                    list.Sort((a, b) =>
                    {
                        int statusCompare = GetStatusPriorityErrorToSuccess(a.Status)
                            .CompareTo(GetStatusPriorityErrorToSuccess(b.Status));

                        return statusCompare != 0 
                            ? statusCompare 
                            : SecondaryAlphabeticalCompare(a, b);
                    });
                    return;
            }

            Comparison<AssetData> comparison = mode switch
            {
                SortMode.PathAtoZ => (a, b) => Compare(GetSortString(a), GetSortString(b)),
                SortMode.PathZtoA => (a, b) => Compare(GetSortString(b), GetSortString(a)),
                SortMode.NameAtoZ => (a, b) => Compare(GetSortString(a), GetSortString(b)),
                SortMode.NameZtoA => (a, b) => Compare(GetSortString(b), GetSortString(a)),
                _ => null
            };

            if (comparison != null)
                list.Sort(comparison);

            return;

            string GetSortString(AssetData assetData)
            {
                return mode switch
                {
                    SortMode.PathAtoZ or SortMode.PathZtoA => assetData.GetAssetPath(),
                    SortMode.NameAtoZ or SortMode.NameZtoA => assetData.GetAssetName(),
                    _ => null
                };
            }
        }

        private static int Compare(
            string a,
            string b)
        {
            if (a == null && b == null)
                return 0;

            if (a == null)
                return -1;

            if (b == null)
                return 1;

            int ia = 0, ib = 0;

            while (ia < a.Length && ib < b.Length)
            {
                char ca = a[ia];
                char cb = b[ib];

                // Compare numeric substrings numerically
                if (char.IsDigit(ca) && char.IsDigit(cb))
                {
                    long va = 0, vb = 0;

                    while (ia < a.Length && char.IsDigit(a[ia]))
                        va = va * 10 + (a[ia++] - '0');

                    while (ib < b.Length && char.IsDigit(b[ib]))
                        vb = vb * 10 + (b[ib++] - '0');

                    int result = va.CompareTo(vb);

                    if (result != 0)
                        return result;

                    continue;
                }

                // Regular character comparison (case-insensitive)
                int diff = char.ToLowerInvariant(ca).CompareTo(char.ToLowerInvariant(cb));

                if (diff != 0)
                    return diff;

                ia++;
                ib++;
            }

            return a.Length.CompareTo(b.Length);
        }
        
        private static int GetStatusPrioritySuccessToError(InjectionStatus status)
        {
            return status switch
            {
                InjectionStatus.Success => 0,
                InjectionStatus.Warning => 1,
                InjectionStatus.Error   => 2,
                InjectionStatus.Unknown => 3,
                _ => 4
            };
        }

        private static int GetStatusPriorityErrorToSuccess(InjectionStatus status)
        {
            return status switch
            {
                InjectionStatus.Error   => 0,
                InjectionStatus.Warning => 1,
                InjectionStatus.Success => 2,
                InjectionStatus.Unknown => 3,
                _ => 4
            };
        }

        private static int SecondaryAlphabeticalCompare(AssetData a, AssetData b)
        {
            int nameCompare = Compare(a.GetAssetName(), b.GetAssetName());
            
            return nameCompare != 0
                ? nameCompare
                : Compare(a.GetAssetPath(), b.GetAssetPath());
        }
    }
}