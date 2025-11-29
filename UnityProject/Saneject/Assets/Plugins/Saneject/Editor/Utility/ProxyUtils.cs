using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Plugins.Saneject.Runtime.Bindings;
using Plugins.Saneject.Runtime.Proxy;
using Plugins.Saneject.Runtime.Scopes;
using Plugins.Saneject.Runtime.Settings;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Plugins.Saneject.Editor.Utility
{
    /// <summary>
    /// Utilities for generating and resolving proxy scripts and assets for Saneject.
    /// </summary>
    public static class ProxyUtils
    {
        /// <summary>
        /// Generates a proxy MonoBehaviour script for the given type if one does not already exist.
        /// </summary>
        public static void GenerateProxyScript(Type concreteType)
        {
            if (concreteType == null) throw new ArgumentNullException(nameof(concreteType));

            if (!typeof(MonoBehaviour).IsAssignableFrom(concreteType))
                throw new ArgumentException($"{concreteType.FullName} is not a MonoBehaviour.", nameof(concreteType));

            if (DoesProxyScriptExist(concreteType))
                return;

            Type[] interfaces = concreteType
                .GetInterfaces()
                .Where(i => i.IsPublic && !i.IsGenericType && i != typeof(IDisposable) && i != typeof(ISerializationCallbackReceiver))
                .Distinct()
                .ToArray();

            if (interfaces.Length == 0)
                throw new InvalidOperationException($"{concreteType.Name} does not implement any public interfaces.");

            string scriptPath = AssetDatabase.FindAssets($"{concreteType.Name} t:MonoScript")
                .Select(AssetDatabase.GUIDToAssetPath)
                .FirstOrDefault(p => Path.GetFileNameWithoutExtension(p) == concreteType.Name);

            if (string.IsNullOrEmpty(scriptPath))
                throw new FileNotFoundException($"Could not locate script file for type {concreteType.FullName}.");

            string folder = Path.GetDirectoryName(scriptPath)?.Replace("\\", "/");
            string ns = concreteType.Namespace ?? "Global";
            string proxyName = concreteType.Name + "Proxy";
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

        /// <summary>
        /// Returns true if a proxy exists for the given type, either compiled or as source in the same folder.
        /// </summary>
        public static bool DoesProxyScriptExist(Type concreteType)
        {
            if (concreteType == null) throw new ArgumentNullException(nameof(concreteType));

            if (!typeof(MonoBehaviour).IsAssignableFrom(concreteType))
                throw new ArgumentException($"{concreteType.FullName} is not a MonoBehaviour.", nameof(concreteType));

            if (HasCompiledProxyTypeFor(concreteType))
                return true;

            string scriptPath = AssetDatabase.FindAssets($"{concreteType.Name} t:MonoScript")
                .Select(AssetDatabase.GUIDToAssetPath)
                .FirstOrDefault(p => Path.GetFileNameWithoutExtension(p) == concreteType.Name);

            if (string.IsNullOrEmpty(scriptPath))
                throw new FileNotFoundException($"Could not locate script file for type {concreteType.FullName}.");

            string folder = Path.GetDirectoryName(scriptPath)?.Replace("\\", "/");

            if (string.IsNullOrEmpty(folder))
                return false;

            string[] monoScriptGuids = AssetDatabase.FindAssets("t:MonoScript", new[] { folder });

            foreach (string guid in monoScriptGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);

                if (!path.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
                    continue;

                string code;

                try
                {
                    code = File.ReadAllText(path);
                }
                catch
                {
                    continue;
                }

                if (SourceDeclaresProxyOf(code, concreteType))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns the first existing proxy asset of the given type, or creates one if none exist.
        /// </summary>
        public static ScriptableObject GetFirstOrCreateProxyAsset(
            Type proxyType,
            out bool createdNew)
        {
            createdNew = false;

            if (proxyType == null) throw new ArgumentNullException(nameof(proxyType));

            if (!typeof(ScriptableObject).IsAssignableFrom(proxyType))
                throw new ArgumentException($"{proxyType.FullName} is not a ScriptableObject.", nameof(proxyType));

            string[] guids = AssetDatabase.FindAssets($"t:{proxyType.Name}");

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                ScriptableObject existing = AssetDatabase.LoadAssetAtPath(path, proxyType) as ScriptableObject;
                if (existing) return existing;
            }

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

            string proxyAssetPath = $"{targetFolder}/{proxyType.Name}.asset";
            ScriptableObject instance = ScriptableObject.CreateInstance(proxyType);
            AssetDatabase.CreateAsset(instance, proxyAssetPath);
            AssetDatabase.SaveAssets();

            Debug.Log($"Created proxy asset at path '{proxyAssetPath}'.", instance);
            createdNew = true;
            return instance;
        }

        /// <summary>
        /// Finds a compiled proxy type for the given concrete type, or null if none exists.
        /// </summary>
        public static Type GetProxyTypeFromConcreteType(Type concreteType)
        {
            if (concreteType == null) return null;

            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type[] types;

                try
                {
                    types = asm.GetTypes();
                }
                catch (ReflectionTypeLoadException e)
                {
                    types = e.Types.Where(t => t != null).ToArray();
                }

                foreach (Type t in types)
                    if (typeof(ScriptableObject).IsAssignableFrom(t) && InheritsProxyOf(t, concreteType))
                        return t;
            }

            return null;
        }

        /// <summary>
        /// Attempts to generate proxy scripts for the specified set of types if they do not already exist.
        /// </summary>
        /// <param name="types">
        /// A set of types for which proxy scripts should be created.
        /// </param>
        /// <returns>
        /// True if proxy scripts were successfully created for any of the provided types; otherwise, false.
        /// </returns>
        public static bool TryCreateProxyScripts(HashSet<Type> types)
        {
            if (types.Count == 0)
                return false;

            types.RemoveWhere(DoesProxyScriptExist);

            if (types.Count == 0)
                return false;

            string scriptsWord = types.Count == 1 ? "script" : "scripts";

            EditorUtility.DisplayDialog($"Saneject: Proxy {scriptsWord} required", $"{types.Count} proxy {scriptsWord} will be created. Afterwards Unity will recompile and stop the current injection pass. Click 'Inject' again after recompilation to complete the injection.", "Got it");

            foreach (Type type in types)
                GenerateProxyScript(type);

            SessionState.SetInt("Saneject.ProxyStubCount", types.Count);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return true;
        }

        /// <summary>
        /// Attempts to create proxy scripts for the specified scopes if they do not already exist.
        /// </summary>
        /// <param name="scopes">A collection of scopes that may contain proxy bindings.</param>
        /// <returns>
        /// True if at least one proxy script was successfully created; otherwise, false.
        /// </returns>
        public static bool TryCreateProxyScripts(IEnumerable<Scope> scopes)
        {
            HashSet<Type> proxyTypes = scopes
                .SelectMany(scope => scope.GetProxyBindings())
                .Select(binding => binding.ConcreteType)
                .ToHashSet();

            return TryCreateProxyScripts(proxyTypes);
        }

        /// <summary>
        /// Attempts to create proxy scripts for all declared proxies in the specified scenes based on their file paths.
        /// </summary>
        /// <param name="scenePaths">A collection of scene file paths for which proxy scripts are to be created.</param>
        /// <returns>
        /// A boolean value indicating whether the operation completed successfully.
        /// Returns true if proxy scripts were successfully created or already existed for all scenes; otherwise, false.
        /// </returns>
        public static bool TryCreateProxyScriptsForScenes(IEnumerable<string> scenePaths)
        {
            HashSet<Type> types = GetSceneProxyTypes(scenePaths);
            return TryCreateProxyScripts(types);
        }

        /// <summary>
        /// Attempts to create proxy scripts for all declared proxies in the given set of prefab file paths.
        /// </summary>
        /// <param name="prefabsPaths">The collection of file paths to prefab assets.</param>
        /// <returns>
        /// Returns true if proxy scripts were successfully created for one or more prefabs,
        /// or false if none were created.
        /// </returns>
        public static bool TryCreateProxyScriptsForPrefabs(IEnumerable<string> prefabsPaths)
        {
            HashSet<Type> types = GetPrefabProxyTypes(prefabsPaths);
            return TryCreateProxyScripts(types);
        }

        /// <summary>
        /// Attempts to generate proxy scripts for all declared proxies associated with the specified scenes and prefabs.
        /// </summary>
        /// <param name="scenePaths">A collection of file paths for the scenes to process when generating proxy scripts.</param>
        /// <param name="prefabsPaths">A collection of file paths for the prefabs to process when generating proxy scripts.</param>
        /// <returns>
        /// True if one or more proxy scripts were successfully created; otherwise, false.
        /// </returns>
        public static bool TryCreateProxyScriptsForScenesAndPrefabs(
            IEnumerable<string> scenePaths,
            IEnumerable<string> prefabsPaths)
        {
            HashSet<Type> sceneTypes = GetSceneProxyTypes(scenePaths);
            HashSet<Type> prefabTypes = GetPrefabProxyTypes(prefabsPaths);
            HashSet<Type> allTypes = new(sceneTypes);
            allTypes.UnionWith(prefabTypes);
            return TryCreateProxyScripts(allTypes);
        }

        private static bool HasCompiledProxyTypeFor(Type concreteType)
        {
            return GetProxyTypeFromConcreteType(concreteType) != null;
        }

        private static bool InheritsProxyOf(
            Type candidate,
            Type concreteType)
        {
            for (Type t = candidate; t != null && t != typeof(object); t = t.BaseType)
            {
                if (!t.IsGenericType) continue;
                Type def = t.GetGenericTypeDefinition();

                if (def == typeof(ProxyObject<>))
                {
                    Type arg = t.GetGenericArguments()[0];
                    if (arg == concreteType) return true;
                }
            }

            return false;
        }

        private static bool SourceDeclaresProxyOf(
            string code,
            Type concreteType)
        {
            if (concreteType.FullName == null)
            {
                Debug.LogError($"Cannot check source code for proxy of {concreteType.Name}: Full name is null.");
                return false;
            }

            string fq = Regex.Escape(concreteType.FullName);
            string nameOnly = Regex.Escape(concreteType.Name);
            fq = fq.Replace(@"\+", @"(\+|\.)");

            string patternFq = @"\:\s*ProxyObject\s*<\s*(global::\s*)?" + fq + @"\s*>";
            string patternNameOnly = @"\:\s*ProxyObject\s*<\s*(global::\s*)?" + nameOnly + @"\s*>";

            return Regex.IsMatch(code, patternFq) || Regex.IsMatch(code, patternNameOnly);
        }

        private static HashSet<Type> GetSceneProxyTypes(IEnumerable<string> scenePaths)
        {
            HashSet<Type> types = new();

            foreach (string path in scenePaths)
            {
                Scene scene = EditorSceneManager.OpenScene(path);
                GameObject[] rootGameObjects = scene.GetRootGameObjects();

                foreach (GameObject root in rootGameObjects)
                {
                    foreach (Scope scope in root.GetComponentsInChildren<Scope>(true))
                    {
                        scope.ConfigureBindings();
                        scope.ValidateBindings();

                        foreach (Binding proxyBinding in scope.GetProxyBindings())
                            types.Add(proxyBinding.ConcreteType);

                        scope.Dispose();
                    }
                }
            }

            return types;
        }

        private static HashSet<Type> GetPrefabProxyTypes(IEnumerable<string> prefabsPaths)
        {
            HashSet<Type> types = new();

            foreach (string path in prefabsPaths)
            {
                GameObject prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                foreach (Scope scope in prefabAsset.GetComponentsInChildren<Scope>(true))
                {
                    scope.ConfigureBindings();
                    scope.ValidateBindings();

                    foreach (Binding proxyBinding in scope.GetProxyBindings())
                        types.Add(proxyBinding.ConcreteType);

                    scope.Dispose();
                }
            }

            return types;
        }
    }
}