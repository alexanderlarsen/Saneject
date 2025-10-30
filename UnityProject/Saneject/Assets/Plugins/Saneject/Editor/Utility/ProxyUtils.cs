using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Plugins.Saneject.Runtime.Bindings;
using Plugins.Saneject.Runtime.Proxy;
using Plugins.Saneject.Runtime.Settings;
using UnityEditor;
using UnityEngine;

namespace Plugins.Saneject.Editor.Util
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

        public static void CreateMissingProxyStubs(
            this IEnumerable<Binding> proxyBindings,
            out bool isProxyCreationPending)
        {
            isProxyCreationPending = false;

            List<Type> typesToCreate = proxyBindings
                .Select(binding => binding.ConcreteType)
                .Where(type => !DoesProxyScriptExist(type)).ToList();

            if (typesToCreate.Count == 0)
                return;

            isProxyCreationPending = true;

            string scriptsWord = typesToCreate.Count == 1 ? "script" : "scripts";

            EditorUtility.DisplayDialog($"Saneject: Proxy {scriptsWord} required", $"{typesToCreate.Count} proxy {scriptsWord} will be created. Afterwards Unity will recompile and stop the current injection pass. Click 'Inject' again after recompilation to complete the injection.", "Got it");

            typesToCreate.ForEach(GenerateProxyScript);
            SessionState.SetInt("Saneject.ProxyStubCount", typesToCreate.Count);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}