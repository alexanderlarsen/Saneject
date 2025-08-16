using System;
using System.Linq;
using System.Reflection;
using Plugins.Saneject.Editor.Util;
using UnityEditor;
using UnityEngine;

namespace Plugins.Saneject.Editor.Menus
{
    /// <summary>
    /// Context menu for generating interface proxy stubs and assets.
    /// Uses ProxyUtils for all heavy lifting. Keeps SessionState so we can finish after a reload.
    /// </summary>
    public static class GenerateInterfaceProxyMenu
    {
        private const string ProxyTypeNameKey = "Saneject.ProxyDialogFlow.ProxyTypeName";

        [MenuItem("Assets/Generate Interface Proxy", true)]
        private static bool GenerateInterfaceProxy_Validate()
        {
            return Selection.activeObject is MonoScript ms
                   && ms.GetClass() is { } t
                   && typeof(MonoBehaviour).IsAssignableFrom(t);
        }

        [MenuItem("Assets/Generate Interface Proxy")]
        private static void GenerateInterfaceProxy()
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
                    "Saneject: No Interfaces",
                    $"{mbType.Name} doesn't implement any public interfaces, which is required for proxy forwarding.",
                    "OK");

                return;
            }

            // Build full proxy type name for post-reload lookup
            string ns = mbType.Namespace ?? "Global";
            string proxyFullName = $"{ns}.{mbType.Name}Proxy";

            // If stub is missing, ask to create it and trigger a compile
            if (!ProxyUtils.DoesProxyStubExist(mbType))
            {
                string interfaceList = string.Join("\n", interfaces.Select(i => i.Name));

                bool gen = EditorUtility.DisplayDialog(
                    "Saneject: Generate Interface Proxy",
                    $"Generate a proxy for '{mbType.Name}' forwarding:\n\n{interfaceList}",
                    "Yes", "No");

                if (!gen) return;

                // Create stub + mark intent, then force a refresh (domain reload)
                ProxyUtils.CreateProxyStub(mbType);
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
            ScriptableObject asset = ProxyUtils.GetOrCreateProxyAsset(proxyType, out bool createdNew);
            Selection.activeObject = asset;
            EditorGUIUtility.PingObject(asset);
            
            string message = createdNew
                ? $"Proxy asset for '{proxyType.Name}' was generated."
                : $"Proxy script and asset for '{proxyType.Name}' already exists.";

            EditorUtility.DisplayDialog(
                "Saneject: Proxy Ready",
                message,
                "OK");
        }
    }
}