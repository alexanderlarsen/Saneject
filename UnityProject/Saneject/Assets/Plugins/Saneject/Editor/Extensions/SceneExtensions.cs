using UnityEngine.SceneManagement;

namespace Plugins.Saneject.Editor.Extensions
{
    public static class SceneExtensions
    {
        public static long GetHandleCompat(this Scene scene)
        {
#if UNITY_6000_4_OR_NEWER
            return (long)scene.GetRawData();
#else
            return scene.handle;
#endif
        }
    }
}