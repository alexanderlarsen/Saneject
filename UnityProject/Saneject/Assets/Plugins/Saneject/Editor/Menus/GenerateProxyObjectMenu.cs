using System;
using System.Linq;
using System.Reflection;
using Plugins.Saneject.Editor.Utility;
using UnityEditor;
using UnityEngine;

namespace Plugins.Saneject.Editor.Menus
{
    /// <summary>
    /// Context menu for generating interface proxy stubs and assets.
    /// Uses ProxyUtils for all heavy lifting. Keeps SessionState so we can finish after a reload.
    /// </summary>
    public static class GenerateProxyObjectMenu
    {
        private const string ProxyTypeNameKey = "Saneject.ProxyDialogFlow.ProxyTypeName";

        [MenuItem("Assets/Generate Proxy Object", true)]
        private static bool GenerateProxyObject_Validate()
        {
            return Selection.activeObject is MonoScript ms
                   && ms.GetClass() is { } t
                   && typeof(MonoBehaviour).IsAssignableFrom(t);
        }

        [MenuItem("Assets/Generate Proxy Object", false)]
        private static void GenerateProxyObject()
        {
            if (Selection.activeObject is not MonoScript script) return;
            Type mbType = script.GetClass();
            if (mbType == null) return;

            // Discover public, non-generic interfaces (same filter used in ProxyUtils)
            Type[] interfaces = mbType.GetInterfaces()
                .Where(i => i.IsPublic && !i.IsGenericType && i != typeof(IDisposable) && i != typeof(ISerializationCallbackReceiver))
                .Distinct()
                .ToArray();

            if (interfaces.Length == 0)
            {
                EditorUtility.DisplayDialog(
                    "Saneject: Generate Proxy Object",
                    $"{mbType.Name} doesn't implement any public interfaces, which is required for proxy forwarding.",
                    "OK");

                return;
            }

            // Build full proxy type name for post-reload lookup
            string ns = mbType.Namespace ?? "Global";
            string proxyFullName = $"{ns}.{mbType.Name}Proxy";

            // If stub is missing, ask to create it and trigger a compile
            if (!ProxyUtils.DoesProxyScriptExist(mbType))
            {
                string interfaceList = string.Join("\n", interfaces.Select(i => i.Name));

                bool gen = EditorUtility.DisplayDialog(
                    "Saneject: Generate Proxy Object",
                    $"Generate a proxy for '{mbType.Name}' forwarding:\n\n{interfaceList}",
                    "Yes", "No");

                if (!gen) return;

                // Create stub + mark intent, then force a refresh (domain reload)
                ProxyUtils.GenerateProxyScript(mbType);
                SessionState.SetString(ProxyTypeNameKey, proxyFullName);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                return;
            }

            // Stub already exists; try to create/reuse the asset immediately
            ProceedToAsset(proxyFullName);
        }

        [InitializeOnLoadMethod]
        private static void AfterReload()
        {
            string proxyTypeName = SessionState.GetString(ProxyTypeNameKey, "");

            if (string.IsNullOrEmpty(proxyTypeName))
                return;

            // Try to finish the asset creation after compilation
            ProceedToAsset(proxyTypeName);
        }

        private static void ProceedToAsset(string proxyTypeName)
        {
            // Clear intent first so a failure here doesn't loop forever
            SessionState.EraseString(ProxyTypeNameKey);

            // Find compiled type (ProxyUtils expects a Type)
            Type proxyType = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a =>
                {
                    try
                    {
                        return a.GetTypes();
                    }
                    catch (ReflectionTypeLoadException e)
                    {
                        return e.Types.Where(t => t != null);
                    }
                })
                .FirstOrDefault(t => t.FullName == proxyTypeName && typeof(ScriptableObject).IsAssignableFrom(t));

            if (proxyType == null)
            {
                // Still compiling or something went wrong; user can run menu again later
                Debug.LogWarning($"Saneject: Proxy type '{proxyTypeName}' not found after reload. Try the menu again once compilation completes.");
                return;
            }

            // Create or reuse the asset via ProxyUtils (stored under Assets/Saneject/Proxies by default)
            ScriptableObject asset = ProxyUtils.GetFirstOrCreateProxyAsset(proxyType, out bool createdNew);

            string message = createdNew
                ? $"Proxy generated for '{proxyType.Name}'."
                : $"Proxy script and asset for '{proxyType.Name}' already exists.";

            EditorUtility.DisplayDialog(
                "Saneject: Generate Proxy Object",
                message,
                "OK");

            if (createdNew)
                AssetDatabase.Refresh();

            Selection.activeObject = asset;
            EditorGUIUtility.PingObject(asset);
        }
    }
}