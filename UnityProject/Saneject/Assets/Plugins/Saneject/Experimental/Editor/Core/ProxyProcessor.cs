using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Plugins.Saneject.Experimental.Editor.Data;
using Plugins.Saneject.Experimental.Editor.Graph.Nodes;
using Plugins.Saneject.Experimental.Editor.Utilities;
using Plugins.Saneject.Experimental.Runtime.Proxy;
using Plugins.Saneject.Experimental.Runtime.Settings;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Experimental.Editor.Core
{
    public static class ProxyProcessor
    {
        public static ProxyCreationResult CreateProxies(InjectionContext context)
        {
            IReadOnlyCollection<Type> concreteTypes = context.Graph
                .EnumerateAllBindingNodes()
                .Where(bindingNode => context.ValidBindings.Contains(bindingNode))
                .Where(bindingNode => bindingNode is ComponentBindingNode { ResolveFromProxy: true })
                .Select(bindingNode => bindingNode.ConcreteType)
                .Where(type => type != null)
                .ToHashSet();

            if (TryCreateProxyStubs(concreteTypes))
                return ProxyCreationResult.DomainReloadRequired;

            CreateMissingProxyAssets(context, concreteTypes);
            return ProxyCreationResult.Ready;
        }

        public static Object ResolveProxyAsset(Type concreteType)
        {
            Type proxyType = FindProxyStubType(concreteType);

            return AssetDatabase
                .FindAssets($"t:{proxyType.Name}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(path => AssetDatabase.LoadAssetAtPath(path, proxyType))
                .FirstOrDefault(asset => asset && asset.GetType() == proxyType);
        }

        private static Type FindProxyStubType(Type concreteType)
        {
            Type stubBaseType = typeof(ProxyObject<>).MakeGenericType(concreteType);
            return TypeCache.GetTypesDerivedFrom(stubBaseType).FirstOrDefault();
        }

        private static bool TryCreateProxyStubs(IReadOnlyCollection<Type> concreteTypes)
        {
            HashSet<Type> missing = concreteTypes
                .Where(TypeIsMissingProxyStub)
                .ToHashSet();

            if (missing.Count == 0)
                return false;

            DialogUtility.DisplayProxyDialog(missing.Count);

            foreach (Type type in missing)
                GenerateProxyStub(type);

            Logger.SaveProxyStubCreationCount(missing.Count);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return true;
        }

        private static bool TypeIsMissingProxyStub(Type concreteType)
        {
            return FindProxyStubType(concreteType) == null;
        }

        private static void CreateMissingProxyAssets(
            InjectionContext context,
            IReadOnlyCollection<Type> concreteTypes)
        {
            string directory = UserSettings.ProxyAssetGenerationFolder;
            Directory.CreateDirectory(directory);

            foreach (Type type in concreteTypes)
            {
                Type proxyType = FindProxyStubType(type);
                string path = $"{directory}/{proxyType.Name}.asset";

                if (AssetDatabase.AssetPathExists(path))
                    continue;

                ScriptableObject instance = ScriptableObject.CreateInstance(proxyType);
                AssetDatabase.CreateAsset(instance, path);
                AssetDatabase.SaveAssets();
                context.RegisterCreatedProxyAsset(instance, path);
            }
        }

        private static void GenerateProxyStub(Type concreteType)
        {
            Directory.CreateDirectory(UserSettings.ProxyAssetGenerationFolder);
            string className = $"{concreteType.Name}Proxy";
            string proxyScriptPath = $"{UserSettings.ProxyAssetGenerationFolder}/{className}.cs";
            File.WriteAllText(proxyScriptPath, BuildProxyStubCode());
            AssetDatabase.ImportAsset(proxyScriptPath, ImportAssetOptions.ForceSynchronousImport);

            return;

            string BuildProxyStubCode()
            {
                return $@"
namespace Plugins.Saneject.Generated.Proxies
{{
    [Plugins.Saneject.Experimental.Runtime.Attributes.GenerateProxyObject]
    public partial class {className} : Plugins.Saneject.Experimental.Runtime.Proxy.ProxyObject<{concreteType.FullName}>
    {{
    }}
}}";
            }
        }
    }
}