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

namespace Plugins.Saneject.Experimental.Editor.RuntimeProxy
{
    [InitializeOnLoad]
    public static class ProxyScriptGenerator
    {
        static ProxyScriptGenerator()
        {
            if (!UserSettings.GenerateProxyScriptsOnDomainReload)
                return;

            HashSet<Type> missing = EnumerateMissingTypes().ToHashSet();

            if (missing.Count == 0)
                return;

            DisplayDialog.ProxyGeneration.Create(missing.Count);
            GenerateScriptsAndSave(missing);
        }

        public static void GenerateMissingProxyScripts()
        {
            HashSet<Type> missing = EnumerateMissingTypes().ToHashSet();

            if (missing.Count == 0)
            {
                DisplayDialog.ProxyGeneration.AlreadyExist();
                return;
            }

            DisplayDialog.ProxyGeneration.Create(missing.Count);
            GenerateScriptsAndSave(missing);
        }

        private static IEnumerable<Type> EnumerateMissingTypes()
        {
            const string manifestFullName = "Saneject.RuntimeProxy.Generator.AssemblyProxyManifest";
            string scopeAssemblyName = typeof(Scope).Assembly.GetName().Name;
            const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Static;

            // Assemblies that reference the Scope's assembly (Saneject.Runtime)
            IEnumerable<Assembly> assemblies = AppDomain
                .CurrentDomain
                .GetAssemblies()
                .Where(asm => asm
                    .GetReferencedAssemblies()
                    .Any(r => r.Name == scopeAssemblyName)
                );

            foreach (Assembly assembly in assemblies)
            {
                FieldInfo proxyTargetsField = assembly
                    .GetType(manifestFullName, throwOnError: false)?
                    .GetField("RequiredProxyTargets", bindingFlags);

                if (proxyTargetsField?.GetValue(null) is Type[] targets)
                    foreach (Type t in targets)
                        if (IsMissingScript(t))
                            yield return t;
            }
        }

        private static bool IsMissingScript(Type concreteType)
        {
            Type stubBaseType = typeof(RuntimeProxy<>).MakeGenericType(concreteType);
            return TypeCache.GetTypesDerivedFrom(stubBaseType).FirstOrDefault() == null;
        }

        private static void GenerateScriptsAndSave(IEnumerable<Type> types)
        {
            foreach (Type type in types)
            {
                Directory.CreateDirectory(UserSettings.ProxyAssetGenerationFolder);
                string className = $"{type.Name}RuntimeProxy";
                string proxyScriptPath = $"{UserSettings.ProxyAssetGenerationFolder}/{className}.cs";
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
            return $@"
namespace Plugins.Saneject.Generated.Proxies
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