using UnityEditor;

namespace Plugins.Saneject.Experimental.Editor.Utils
{
    public static class DialogUtility
    {
        public static void DisplayProxyDialog(int proxyCount)
        {
            string scriptsWord = proxyCount == 1 ? "script" : "scripts";

            EditorUtility.DisplayDialog
            (
                title: $"Saneject: Proxy {scriptsWord} required",
                message: $"{proxyCount} proxy {scriptsWord} will be created. Afterwards Unity will recompile and stop the current injection pass. Click 'Inject' again after recompilation to complete the injection.",
                ok: "Got it"
            );
        }
    }
}