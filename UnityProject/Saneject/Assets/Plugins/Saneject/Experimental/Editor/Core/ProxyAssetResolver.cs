using System;
using System.IO;
using System.Linq;
using Plugins.Saneject.Experimental.Editor.Data;
using Plugins.Saneject.Experimental.Runtime.Proxy;
using Plugins.Saneject.Experimental.Runtime.Settings;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Experimental.Editor.Core
{
    public static class ProxyAssetResolver
    {
        public static Object Resolve(
            Type concreteType,
            InjectionContext context)
        {
            Type proxyType = FindProxyStubType(concreteType);

            if (proxyType == null)
                return null;

            Object existing = FindExistingProxyAsset(proxyType);

            return existing
                ? existing
                : CreateProxyAsset(proxyType, context);
        }

        private static Type FindProxyStubType(Type concreteType)
        {
            Type stubBaseType = typeof(RuntimeProxy<>).MakeGenericType(concreteType);
            return TypeCache.GetTypesDerivedFrom(stubBaseType).FirstOrDefault();
        }

        private static Object FindExistingProxyAsset(Type proxyType)
        {
            return AssetDatabase
                .FindAssets($"t:{proxyType.Name}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(path => AssetDatabase.LoadAssetAtPath(path, proxyType))
                .FirstOrDefault(asset => asset && asset.GetType() == proxyType);
        }

        private static Object CreateProxyAsset(
            Type proxyType,
            InjectionContext context)
        {
            string directory = UserSettings.ProxyAssetGenerationFolder;
            string path = $"{directory}/{proxyType.Name}.asset";

            Directory.CreateDirectory(directory);
            ScriptableObject instance = ScriptableObject.CreateInstance(proxyType);
            AssetDatabase.CreateAsset(instance, path);
            AssetDatabase.SaveAssets();
            context?.RegisterCreatedProxyAsset(instance, path);

            return instance;
        }
    }
}