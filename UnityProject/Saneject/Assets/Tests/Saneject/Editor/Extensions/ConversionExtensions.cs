using UnityEngine;

namespace Tests.Saneject.Editor.Extensions
{
    public static class ConversionExtensions
    {
        public static GameObject[] ToArray(this GameObject gameObject)
        {
            return new[]
            {
                gameObject
            };
        }
        
        public static Transform[] ToArray(this Transform transform)
        {
            return new[]
            {
                transform
            };
        }
    }
}