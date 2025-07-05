using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Plugins.Saneject.Editor.Menus
{
    /// <summary>
    /// Provides a context menu item for generating interface proxy classes and assets for <see cref="MonoBehaviour" /> types.
    /// Handles both proxy C# code generation and corresponding <see cref="ScriptableObject" /> proxy asset creation.
    /// </summary>
    public static class GenerateInterfaceProxyMenu
    {
        private const string ProxyTypeNameKey = "ProxyDialogFlow.ProxyTypeName";
        private const string ProxyAssetPathKey = "ProxyDialogFlow.ProxyAssetPath";

        /// <summary>
        /// Validates whether the "Generate Interface Proxy" menu item should be enabled.
        /// Only enabled for <see cref="MonoScript" /> objects that represent a <see cref="MonoBehaviour" /> with public interfaces.
        /// </summary>
        [MenuItem("Assets/Generate Interface Proxy", true)]
        private static bool GenerateInterfaceProxy_Validate()
        {
            return Selection.activeObject is MonoScript ms &&
                   ms.GetClass() is Type t &&
                   typeof(MonoBehaviour).IsAssignableFrom(t);
        }

        /// <summary>
        /// Displays dialogs to create an interface proxy class and a corresponding <see cref="ScriptableObject" /> asset.
        /// Handles class creation, interface discovery, and asset instantiation.
        /// </summary>
        [MenuItem("Assets/Generate Interface Proxy")]
        private static void GenerateInterfaceProxy()
        {
            MonoScript script = (MonoScript)Selection.activeObject;
            Type mbType = script.GetClass();
            if (mbType == null) return;

            string ns = mbType.Namespace ?? "Global";
            string proxyName = mbType.Name + "Proxy";
            string proxyFullName = ns + "." + proxyName;
            string scriptPath = AssetDatabase.GetAssetPath(script);
            string folder = Path.GetDirectoryName(scriptPath).Replace("\\", "/");
            string proxyScriptPath = $"{folder}/{proxyName}.cs";
            string proxyAssetPath = $"{folder}/{proxyName}.asset";

            // Check for interfaces (for dialog)
            Type[] interfaces = mbType.GetInterfaces()
                .Where(i => i.IsPublic && !i.IsGenericType && i != typeof(IDisposable) && i != typeof(ISerializationCallbackReceiver))
                .Distinct().ToArray();

            if (interfaces.Length == 0)
            {
                EditorUtility.DisplayDialog(
                    "No Interfaces",
                    $"{mbType.Name} doesn't implement any public interfaces.",
                    "OK");

                return;
            }

            string interfaceList = string.Join("\n", interfaces.Select(i => i.Name));

            // Step 1: Ask to create class if not exists
            if (!File.Exists(proxyScriptPath))
            {
                bool gen = EditorUtility.DisplayDialog(
                    "Generate Interface Proxy",
                    $"Generate a proxy for '{mbType.Name}' forwarding:\n\n{interfaceList}",
                    "Yes", "No");

                if (!gen) return;

                string code =
                    $@"using Plugins.Saneject.Runtime.Attributes;
using Plugins.Saneject.Runtime.InterfaceProxy;

namespace {ns}
{{
    [GenerateInterfaceProxy]
    public partial class {proxyName} : InterfaceProxyObject<{mbType.FullName}>
    {{
    }}
}}";

                File.WriteAllText(proxyScriptPath, code);
                AssetDatabase.Refresh();

                // Store intent for after compilation
                SessionState.SetString(ProxyTypeNameKey, proxyFullName);
                SessionState.SetString(ProxyAssetPathKey, proxyAssetPath);

                return;
            }

            // If class exists, proceed as before
            ProceedToAssetDialog(proxyFullName, proxyAssetPath);
        }

        [InitializeOnLoadMethod]
        private static void AfterReload()
        {
            string proxyTypeName = SessionState.GetString(ProxyTypeNameKey, "");
            string proxyAssetPath = SessionState.GetString(ProxyAssetPathKey, "");

            if (string.IsNullOrEmpty(proxyTypeName) || string.IsNullOrEmpty(proxyAssetPath))
                return;

            // Try to find compiled type
            Type proxyType = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .FirstOrDefault(t => t.FullName == proxyTypeName && typeof(ScriptableObject).IsAssignableFrom(t));

            if (proxyType != null)
                ProceedToAssetDialog(proxyTypeName, proxyAssetPath);
        }

        private static void ProceedToAssetDialog(
            string proxyTypeName,
            string proxyAssetPath)
        {
            // Find the new type
            Type proxyType = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .FirstOrDefault(t => t.FullName == proxyTypeName && typeof(ScriptableObject).IsAssignableFrom(t));

            if (proxyType == null)
                return; // Still not compiled

            // Check for asset
            bool assetExists = File.Exists(proxyAssetPath);

            // Clean up state
            SessionState.EraseString(ProxyTypeNameKey);
            SessionState.EraseString(ProxyAssetPathKey);

            if (assetExists)
            {
                EditorUtility.DisplayDialog(
                    "Asset Already Exists",
                    "ScriptableObject asset already exists for this proxy class.",
                    "OK");

                return;
            }

            // Ask to create asset
            if (EditorUtility.DisplayDialog(
                    "Create Proxy Asset",
                    $"Create a ScriptableObject asset for '{proxyType.Name}'?",
                    "Yes", "No"))
            {
                ScriptableObject inst = ScriptableObject.CreateInstance(proxyType);
                AssetDatabase.CreateAsset(inst, proxyAssetPath);
                AssetDatabase.SaveAssets();
                Selection.activeObject = inst;

                EditorUtility.DisplayDialog(
                    "Asset Created",
                    "ScriptableObject asset created.",
                    "OK");
            }
        }
    }
}