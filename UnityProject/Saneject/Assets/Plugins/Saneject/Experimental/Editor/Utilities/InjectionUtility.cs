using Plugins.Saneject.Experimental.Editor.Data.Context;
using Plugins.Saneject.Experimental.Editor.Pipeline;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Plugins.Saneject.Experimental.Editor.Utilities
{
    public static class InjectionUtility
    {
        public static void InjectCurrentScene()
        {
            if (!DialogUtility.InjectionMenus.Confirm_Inject_CurrentScene())
                return;
            
            GameObject[] startObjects =
                SceneManager
                    .GetActiveScene()
                    .GetRootGameObjects();

            InjectionRunner.Run(startObjects, ContextWalkFilter.All);
        }
    }
}