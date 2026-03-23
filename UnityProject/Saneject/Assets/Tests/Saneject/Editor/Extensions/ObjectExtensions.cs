using System.Collections.Generic;
using UnityEngine;

namespace Tests.Saneject.Editor.Extensions
{
    public static class ObjectExtensions
    {
        public static IEnumerable<T> ToArray<T>(this T obj) where T : Object
        {
            return new[]
            {
                obj
            };
        }
    }
}