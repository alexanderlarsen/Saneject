using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Plugins.Saneject.Runtime.Settings;
using UnityEditor;
using UnityEngine;

namespace Plugins.Saneject.Editor.Util
{
    public static class ProxyUtils
    {
        public static void GenerateProxyScript(Type concreteType)
        {
            if (concreteType == null)
                throw new ArgumentNullException(nameof(concreteType));

            if (!typeof(MonoBehaviour).IsAssignableFrom(concreteType))
                throw new ArgumentException($"{concreteType.FullName} is not a MonoBehaviour.", nameof(concreteType));

            // Collect relevant public interfaces
            Type[] interfaces = concreteType
                .GetInterfaces()
                .Where(i => i.IsPublic && !i.IsGenericType && i != typeof(IDisposable) && i != typeof(ISerializationCallbackReceiver))
                .Distinct()
                .ToArray();

            if (interfaces.Length == 0)
                throw new InvalidOperationException($"{concreteType.Name} does not implement any public interfaces.");

            string ns = concreteType.Namespace ?? "Global";
            string proxyName = concreteType.Name + "Proxy";

            // Locate the folder of the original script
            string scriptPath = AssetDatabase.FindAssets($"{concreteType.Name} t:MonoScript")
                .Select(AssetDatabase.GUIDToAssetPath)
                .FirstOrDefault(p => Path.GetFileNameWithoutExtension(p) == concreteType.Name);

            if (string.IsNullOrEmpty(scriptPath))
                throw new FileNotFoundException($"Could not locate script file for type {concreteType.FullName}.");

            string folder = Path.GetDirectoryName(scriptPath)?.Replace("\\", "/");
            string proxyScriptPath = $"{folder}/{proxyName}.cs";

            if (File.Exists(proxyScriptPath))
                return;

            string code =
                $@"using Plugins.Saneject.Runtime.Attributes;
using Plugins.Saneject.Runtime.Proxy;

namespace {ns}
{{
    [GenerateProxyObject]
    public partial class {proxyName} : ProxyObject<{concreteType.FullName}>
    {{
    }}
}}";

            File.WriteAllText(proxyScriptPath, code);
            AssetDatabase.ImportAsset(proxyScriptPath, ImportAssetOptions.ForceSynchronousImport);
        }

        public static bool DoesProxyScriptExist(Type concreteType)
        {
            if (concreteType == null)
                throw new ArgumentNullException(nameof(concreteType));

            if (!typeof(MonoBehaviour).IsAssignableFrom(concreteType))
                throw new ArgumentException($"{concreteType.FullName} is not a MonoBehaviour.", nameof(concreteType));

            string proxyName = concreteType.Name + "Proxy";

            // Locate the folder of the original script
            string scriptPath = AssetDatabase.FindAssets($"{concreteType.Name} t:MonoScript")
                .Select(AssetDatabase.GUIDToAssetPath)
                .FirstOrDefault(p => Path.GetFileNameWithoutExtension(p) == concreteType.Name);

            if (string.IsNullOrEmpty(scriptPath))
                throw new FileNotFoundException($"Could not locate script file for type {concreteType.FullName}.");

            string folder = Path.GetDirectoryName(scriptPath)?.Replace("\\", "/");
            string proxyScriptPath = $"{folder}/{proxyName}.cs";

            return File.Exists(proxyScriptPath);
        }

        public static ScriptableObject GetFirstOrCreateProxyAsset(
            Type proxyType,
            out bool createdNew)
        {
            createdNew = false;

            if (proxyType == null)
                throw new ArgumentNullException(nameof(proxyType));

            if (!typeof(ScriptableObject).IsAssignableFrom(proxyType))
                throw new ArgumentException($"{proxyType.FullName} is not a ScriptableObject.", nameof(proxyType));

            // Search for an existing asset of this type anywhere in the project
            string[] guids = AssetDatabase.FindAssets($"t:{proxyType.Name}");

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                ScriptableObject existing = AssetDatabase.LoadAssetAtPath(path, proxyType) as ScriptableObject;

                if (existing)
                    return existing;
            }

            // Ensure folder exists
            string targetFolder = UserSettings.ProxyAssetGenerationFolder.TrimEnd('/');

            if (!AssetDatabase.IsValidFolder(targetFolder))
            {
                string[] parts = targetFolder.Split('/');
                string currentPath = parts[0];

                for (int i = 1; i < parts.Length; i++)
                {
                    string nextPath = $"{currentPath}/{parts[i]}";

                    if (!AssetDatabase.IsValidFolder(nextPath))
                        AssetDatabase.CreateFolder(currentPath, parts[i]);

                    currentPath = nextPath;
                }
            }

            // Create new asset in user-defined folder
            string proxyAssetPath = $"{targetFolder}/{proxyType.Name}.asset";
            ScriptableObject instance = ScriptableObject.CreateInstance(proxyType);
            AssetDatabase.CreateAsset(instance, proxyAssetPath);
            AssetDatabase.SaveAssets();

            Debug.Log($"Created proxy asset at path '{proxyAssetPath}'.", instance);
            createdNew = true;
            return instance;
        }

        public static Type GetProxyTypeFromConcreteType(Type concreteType)
        {
            string ns = concreteType.Namespace ?? "Global";
            string proxyFullName = ns + "." + concreteType.Name + "Proxy";

            return AppDomain.CurrentDomain.GetAssemblies()
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
                .FirstOrDefault(t => t.FullName == proxyFullName && typeof(ScriptableObject).IsAssignableFrom(t));
        }
    }
}