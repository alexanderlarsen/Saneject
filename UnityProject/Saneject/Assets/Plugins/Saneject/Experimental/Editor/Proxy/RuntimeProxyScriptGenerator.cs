using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Plugins.Saneject.Experimental.Editor.Utilities;
using Plugins.Saneject.Experimental.Runtime.Proxy;
using Plugins.Saneject.Experimental.Runtime.Scopes;
using Plugins.Saneject.Experimental.Runtime.Settings;
using UnityEditor;

// ReSharper disable LoopCanBePartlyConvertedToQuery

namespace Plugins.Saneject.Experimental.Editor.Proxy
{
    [InitializeOnLoad]
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
                string className = $"{type.Name}RuntimeProxy";
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
    [Plugins.Saneject.Experimental.Runtime.Attributes.GenerateRuntimeProxy]
    public partial class {className} : Plugins.Saneject.Experimental.Runtime.Proxy.RuntimeProxy<{typeFullName}>
    {{
    }}
}}
";
        }
    }
} 