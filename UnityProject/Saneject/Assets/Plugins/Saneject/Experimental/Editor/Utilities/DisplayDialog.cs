using System.Text;
using Plugins.Saneject.Experimental.Runtime.Settings;
using UnityEditor;

namespace Plugins.Saneject.Experimental.Editor.Utilities
{
    public static class DisplayDialog
    {
        public static class ProxyGeneration
        {
            public static void Create(int proxyCount)
            {
                EditorUtility.DisplayDialog(
                    title: "Saneject: Runtime Proxy Generation",
                    message:
                    $"{proxyCount} of your FromRuntimeProxy() bindings {(proxyCount == 1 ? "needs a proxy script" : "need proxy scripts")}.\n\n" +
                    $"{(proxyCount == 1 ? "It" : "They")} will be generated during this domain reload and saved to:\n\n" +
                    $"{UserSettings.ProxyAssetGenerationFolder}\n\n" +
                    "You can disable automatic proxy generation in the Saneject settings and run it manually from the Saneject menu instead.",
                    ok: "Got it"
                );
            }

            public static void AlreadyExist()
            {
                EditorUtility.DisplayDialog(
                    title: "Saneject: Runtime Proxy Generation",
                    message: "All necessary runtime proxy scripts already exist.",
                    ok: "Got it"
                );
            }
        }

        public static class BatchInjection
        {
            public static bool UserConfirmed(
                int sceneCount,
                int prefabCount)
            {
                StringBuilder messageBuilder = new();

                messageBuilder.Append("Your selection includes ");
                
                if (sceneCount > 0)
                {
                    messageBuilder.Append($"{sceneCount} {(sceneCount == 1 ? "scene" : "scenes")}");
                    
                    if (prefabCount > 0)
                        messageBuilder.Append(" and ");
                }

                if (prefabCount > 0)
                {
                    messageBuilder.Append($"{prefabCount} {(prefabCount == 1 ? "prefab" : "prefabs")}");
                }

                messageBuilder.Append(".");
                messageBuilder.AppendLine();
                messageBuilder.AppendLine();
                messageBuilder.AppendLine("A batch injection operation will be performed on all selected assets."); 
                messageBuilder.AppendLine();
                messageBuilder.Append("Do you want to continue?");

                return EditorUtility.DisplayDialog(
                    "Saneject: Inject Selected Assets",
                    messageBuilder.ToString(),
                    "Inject",
                    "Cancel"
                );
            }
        }
    }
}