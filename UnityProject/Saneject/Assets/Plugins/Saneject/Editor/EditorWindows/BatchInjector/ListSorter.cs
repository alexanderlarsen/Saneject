﻿using System;
using System.Collections.Generic;
using System.IO;

namespace Plugins.Saneject.Editor.EditorWindows.BatchInjector
{
    public static class ListSorter
    {
        public static void SortList<T>(
            List<T> list,
            SortMode mode,
            Func<T, string> pathSelector,
            bool isNameSort = false)
        {
            if (mode == SortMode.Custom || list is not { Count: > 1 })
                return;

            Comparison<T> comparison = mode switch
            {
                SortMode.PathAtoZ => (
                    a,
                    b) => NaturalSortComparer.Compare(Key(a), Key(b)),
                SortMode.PathZtoA => (
                    a,
                    b) => NaturalSortComparer.Compare(Key(b), Key(a)),
                SortMode.NameAtoZ => (
                    a,
                    b) => NaturalSortComparer.Compare(Key(a), Key(b)),
                SortMode.NameZtoA => (
                    a,
                    b) => NaturalSortComparer.Compare(Key(b), Key(a)),
                _ => null
            };

            if (comparison != null)
                list.Sort(comparison);

            return;

            string Key(T item)
            {
                string path = pathSelector(item);
                return !isNameSort ? path : Path.GetFileNameWithoutExtension(path);
            }
        }

        private static class NaturalSortComparer
        {
            public static int Compare(
                string a,
                string b)
            {
                if (a == null && b == null) return 0;
                if (a == null) return -1;
                if (b == null) return 1;

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
}