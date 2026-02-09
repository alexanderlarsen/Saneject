using System;
using System.IO;
using System.Linq;
using System.Text;
using Plugins.Saneject.Experimental.Editor.Data.Injection;
using Plugins.Saneject.Experimental.Runtime.Bindings.Component;
using Plugins.Saneject.Experimental.Runtime.Proxy;
using Plugins.Saneject.Experimental.Runtime.Settings;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Experimental.Editor.Pipeline
{
    public static class ProxyAssetResolver
    {
        public static Object Resolve(
            Type concreteType,
            RuntimeProxyConfig config,
            InjectionContext context)
        {
            Type proxyType = FindProxyStubType(concreteType);

            if (proxyType == null)
                return null;

            Object existing = FindExistingProxyAsset
            (
                proxyType,
                config
            );

            return existing
                ? existing
                : CreateProxyAsset
                (
                    proxyType,
                    config,
                    context
                );
        }

        private static Type FindProxyStubType(Type concreteType)
        {
            Type stubBaseType = typeof(RuntimeProxy<>).MakeGenericType(concreteType);
            return TypeCache.GetTypesDerivedFrom(stubBaseType).FirstOrDefault();
        }

        private static Object FindExistingProxyAsset(
            Type proxyType,
            RuntimeProxyConfig config)
        {
            return AssetDatabase
                .FindAssets($"t:{proxyType.Name}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(path => AssetDatabase.LoadAssetAtPath(path, proxyType))
                .Where(obj => ((RuntimeProxyBase)obj).HasConfig(config))
                .FirstOrDefault(asset => asset && asset.GetType() == proxyType);
        }

        private static Object CreateProxyAsset(
            Type proxyType,
            RuntimeProxyConfig config,
            InjectionContext context)
        {
            string directory = ProjectSettings.ProxyAssetGenerationFolder;
            string basePath = $"{directory}/{BuildProxyAssetName(proxyType, config)}.asset";
            string uniquePath = AssetDatabase.GenerateUniqueAssetPath(basePath);

            Directory.CreateDirectory(directory);
            ScriptableObject instance = ScriptableObject.CreateInstance(proxyType);
            ((RuntimeProxyBase)instance).AssignConfig(config);
            AssetDatabase.CreateAsset(instance, uniquePath);
            AssetDatabase.SaveAssets();
            context?.RegisterCreatedProxyAsset(instance, uniquePath);
            return instance;
        }

        private static string BuildProxyAssetName(
            Type proxyType,
            RuntimeProxyConfig config)
        {
            StringBuilder sb = new();
            sb.Append(proxyType.Name);
            sb.Append(" (");
            sb.Append(config.ResolveMethod);

            if (config.Prefab)
                sb.Append($": {config.Prefab.name}");

            if (config.DontDestroyOnLoad)
                sb.Append(", DDOL");

            sb.Append(")");
            return sb.ToString();
        }
    }
}