using System;
using System.Collections.Generic;

namespace Plugins.Saneject.Editor.BatchInjection
{
    public static class AssetListSorter
    {
        public static void SortList(
            List<AssetItem> list,
            SortMode mode)
        {
            if (mode == SortMode.Custom || list is not { Count: > 1 })
                return;

            switch (mode)
            {
                case SortMode.DisabledToEnabled:
                    list.Sort((
                        a,
                        b) =>
                    {
                        int flagCompare = a.Enabled.CompareTo(b.Enabled);

                        if (flagCompare != 0)
                            return flagCompare;

                        // Secondary alphabetical sort (by Name, then Path)
                        int nameCompare = Compare(a.Name, b.Name);
                        return nameCompare != 0 ? nameCompare : Compare(a.Path, b.Path);
                    });

                    return;

                case SortMode.EnabledToDisabled:
                    list.Sort((
                        a,
                        b) =>
                    {
                        int flagCompare = b.Enabled.CompareTo(a.Enabled);

                        if (flagCompare != 0)
                            return flagCompare;

                        // Secondary alphabetical sort (by Name, then Path)
                        int nameCompare = Compare(a.Name, b.Name);
                        return nameCompare != 0 ? nameCompare : Compare(a.Path, b.Path);
                    });

                    return;
            }

            Comparison<AssetItem> comparison = mode switch
            {
                SortMode.PathAtoZ => (
                    a,
                    b) => Compare(GetSortString(a), GetSortString(b)),
                SortMode.PathZtoA => (
                    a,
                    b) => Compare(GetSortString(b), GetSortString(a)),
                SortMode.NameAtoZ => (
                    a,
                    b) => Compare(GetSortString(a), GetSortString(b)),
                SortMode.NameZtoA => (
                    a,
                    b) => Compare(GetSortString(b), GetSortString(a)),
                _ => null
            };

            if (comparison != null)
                list.Sort(comparison);

            return;

            string GetSortString(AssetItem item)
            {
                return mode switch
                {
                    SortMode.PathAtoZ or SortMode.PathZtoA => item.Path,
                    SortMode.NameAtoZ or SortMode.NameZtoA => item.Name,
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
    }
}