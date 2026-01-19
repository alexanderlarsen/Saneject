using Plugins.Saneject.Experimental.Runtime.Settings;
using UnityEditor;

namespace Plugins.Saneject.Experimental.Editor.Utilities
{
    public static class DialogUtility
    {
        public static class ProxyGeneration
        {
            public static void DisplayCreationDialog(int proxyCount)
            {
                EditorUtility.DisplayDialog(
                    title: "Saneject: Runtime proxy generation",
                    message:
                    $"{proxyCount} of your FromRuntimeProxy() bindings {(proxyCount == 1 ? "needs a proxy script" : "need proxy scripts")}.\n\n" +
                    $"{(proxyCount == 1 ? "It" : "They")} will be generated during this domain reload and saved to:\n\n" +
                    $"{UserSettings.ProxyAssetGenerationFolder}\n\n" +
                    "You can disable automatic proxy generation in the Saneject settings and run it manually from the Saneject menu instead.",
                    ok: "Got it"
                );
            }

            public static void DisplayAlreadyExistDialog()
            {
                EditorUtility.DisplayDialog(
                    title: "Saneject: Runtime proxy generation",
                    message: "All necessary runtime proxy scripts already exist.",
                    ok: "Got it"
                );
            }
        }
    }
}