using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Plugins.Saneject.Experimental.Editor.Data;
using Plugins.Saneject.Experimental.Editor.Graph.Nodes;
using Plugins.Saneject.Experimental.Editor.Utils;
using Plugins.Saneject.Runtime.Proxy;
using Plugins.Saneject.Runtime.Settings;
using UnityEditor;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Editor.Core
{
    public static class ProxyProcessor
    {
        public static ProxyCreationResult CreateProxies(InjectionSession session)
        {
            IReadOnlyCollection<Type> concreteTypes = session.Graph
                .EnumerateAllBindingNodes()
                .Where(bindingNode => session.ValidBindings.Contains(bindingNode))
                .Where(bindingNode => bindingNode is ComponentBindingNode { ResolveFromProxy: true })
                .Select(bindingNode => bindingNode.ConcreteType)
                .Where(type => type != null)
                .ToHashSet();

            if (TryCreateProxyStubs(concreteTypes))
                return ProxyCreationResult.DomainReloadRequired;

            CreateMissingProxyAssets(session, concreteTypes);
            return ProxyCreationResult.Ready;
        }

        public static Type FindProxyStubType(Type concreteType)
        {
            return concreteType != null
                ? AppDomain.CurrentDomain
                    .GetAssemblies()
                    .SelectMany(GetNonNullTypes)
                    .FirstOrDefault(t => typeof(ScriptableObject).IsAssignableFrom(t) && InheritsProxyOf(t, concreteType))
                : throw new ArgumentNullException(nameof(concreteType));

            static IEnumerable<Type> GetNonNullTypes(Assembly a)
            {
                try
                {
                    return a.GetTypes();
                }
                catch (ReflectionTypeLoadException e)
                {
                    return e.Types.Where(t => t != null);
                }
            }

            static bool InheritsProxyOf(
                Type proxyCandidate,
                Type genericType)
            {
                for (Type t = proxyCandidate; t != null && t != typeof(object); t = t.BaseType)
                {
                    if (!t.IsGenericType)
                        continue;

                    Type def = t.GetGenericTypeDefinition();

                    if (def == typeof(ProxyObject<>))
                    {
                        Type arg = t.GetGenericArguments()[0];

                        if (arg == genericType)
                            return true;
                    }
                }

                return false;
            }
        }

        private static bool TryCreateProxyStubs(IReadOnlyCollection<Type> concreteTypes)
        {
            HashSet<Type> missing = concreteTypes
                .Where(type => FindProxyStubType(type) == null)
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

        private static void CreateMissingProxyAssets(
            InjectionSession session,
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
                session.RegisterCreatedProxyAsset(instance, path);
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
using Plugins.Saneject.Runtime.Attributes;
using Plugins.Saneject.Runtime.Proxy;

namespace Plugins.Saneject.Generated.Proxies
{{
    [GenerateProxyObject]
    public partial class {className} : ProxyObject<{concreteType.FullName}>
    {{
    }}
}}
";
            }
        }
    }
}