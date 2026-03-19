using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Plugins.Saneject.Editor.Utilities;
using Plugins.Saneject.Runtime.Proxy;
using Plugins.Saneject.Runtime.Settings;
using UnityEditor;

// ReSharper disable LoopCanBePartlyConvertedToQuery

namespace Plugins.Saneject.Editor.Proxy
{
    [EditorBrowsable(EditorBrowsableState.Never), InitializeOnLoad]
    public static class RuntimeProxyScriptGenerator
    {
        static RuntimeProxyScriptGenerator()
        {
            if (!ProjectSettings.GenerateProxyScriptsOnDomainReload)
                return;

            HashSet<Type> missing = RuntimeProxyManifestUtility
                .EnumerateManifestTypes()
                .Where(IsMissingScript)
                .ToHashSet();

            if (missing.Count == 0)
                return;

            DialogUtility.ProxyGeneration.Display_ProxyCreate(missing.Count);
            GenerateScriptsAndSave(missing);
        }

        public static void GenerateMissingProxyScripts()
        {
            // HashSet<Type> missing = EnumerateMissingTypes().ToHashSet();
            HashSet<Type> missing = RuntimeProxyManifestUtility
                .EnumerateManifestTypes()
                .Where(IsMissingScript)
                .ToHashSet();

            if (missing.Count == 0)
            {
                DialogUtility.ProxyGeneration.Display_ProxyAlreadyExist();
                return;
            }

            DialogUtility.ProxyGeneration.Display_ProxyCreate(missing.Count);
            GenerateScriptsAndSave(missing);
        }

        private static bool IsMissingScript(Type concreteType)
        {
            Type scriptBaseType = typeof(RuntimeProxy<>).MakeGenericType(concreteType);
            return TypeCache.GetTypesDerivedFrom(scriptBaseType).FirstOrDefault() == null;
        }

        private static void GenerateScriptsAndSave(IEnumerable<Type> types)
        {
            foreach (Type type in types)
            {
                Directory.CreateDirectory(ProjectSettings.ProxyAssetGenerationFolder);
                string className = $"{type.Name}Proxy";

                if (!string.IsNullOrWhiteSpace(type.Namespace))
                    className += $"{StableHash(type.FullName)}";

                string proxyScriptPath = $"{ProjectSettings.ProxyAssetGenerationFolder}/{className}.cs";
                File.WriteAllText(proxyScriptPath, GetScriptCode(className, type.FullName));
                AssetDatabase.ImportAsset(proxyScriptPath, ImportAssetOptions.ForceSynchronousImport);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static string GetScriptCode(
            string className,
            string typeFullName)
        {
            typeFullName = typeFullName.Replace("+", ".");

            return $@"
{NamespaceUtility.GetNamespaceFromAssetsRelativePath(ProjectSettings.ProxyAssetGenerationFolder)}
{{
    /// <summary>
    /// Auto-generated runtime proxy for a concrete Component type.
    /// Acts as a serialized placeholder that resolves to the real instance at runtime
    /// according to its resolve strategy. The appended string is a deterministic hash
    /// of the full original type name, ensuring a unique proxy type when multiple
    /// classes share the same simple name.
    /// </summary>
    [Plugins.Saneject.Runtime.Attributes.GenerateRuntimeProxy]
    public partial class {className} : Plugins.Saneject.Runtime.Proxy.RuntimeProxy<{typeFullName}>
    {{
    }}
}}
";
        }

        private static string StableHash(string input)
        {
            using SHA1 sha1 = SHA1.Create();

            byte[] bytes = Encoding.UTF8.GetBytes(input);
            byte[] hash = sha1.ComputeHash(bytes);

            // First 4 bytes = 8 hex chars
            StringBuilder sb = new(8);

            for (int i = 0; i < 4; i++)
                sb.Append(hash[i].ToString("X2"));

            return sb.ToString();
        }
    }
}