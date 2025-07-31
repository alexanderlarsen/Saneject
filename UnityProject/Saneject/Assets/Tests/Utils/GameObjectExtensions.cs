using UnityEngine;

namespace Tests.Utils
{
    public static class GameObjectExtensions
    {
        public static void DestroyImmediate(this Object obj)
        {
            Object.DestroyImmediate(obj);
        }
    }
}