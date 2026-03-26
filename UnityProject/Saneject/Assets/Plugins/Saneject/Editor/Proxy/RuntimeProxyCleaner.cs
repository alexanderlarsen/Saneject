using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Plugins.Saneject.Editor.Utilities;
using Plugins.Saneject.Runtime.Attributes;
using Plugins.Saneject.Runtime.Proxy;
using Plugins.Saneject.Runtime.Settings;
using UnityEditor;
using UnityEngine;

// ReSharper disable LoopCanBeConvertedToQuery

namespace Plugins.Saneject.Editor.Proxy
{
    [EditorBrowsable(EditorBrowsableState.Never), InitializeOnLoad]
    public static class RuntimeProxyCleaner
    {
        static RuntimeProxyCleaner()
        {
            if (!UserSettings.LogUnusedRuntimeProxiesOnDomainReload)
                return;

            HashSet<Type> unusedTypes = GetUnusedTypes();
            HashSet<string> unusedAssetPaths = GetUnusedAssetPaths(unusedTypes);

            if (unusedTypes.Count == 0 && unusedAssetPaths.Count == 0)
                return;

            string unusedTypeList = unusedTypes.Count == 0
                ? "None"
                : string.Join(", ", unusedTypes.Select(t => t.Name));

            string unusedAssetSummary = unusedAssetPaths.Count == 0
                ? "None"
                : string.Join
                (
                    ", ",
                    unusedAssetPaths
                        .Select(AssetDatabase.LoadAssetAtPath<RuntimeProxyBase>)
                        .Where(asset => asset != null)
                        .GroupBy(asset => asset.GetType().Name)
                        .OrderBy(group => group.Key)
                        .Select(group => $"{group.Key} ({group.Count()} unused)")
                );

            Debug.LogWarning
            (
                $"Saneject: Found {unusedTypes.Count} unused runtime proxy {(unusedTypes.Count == 1 ? "type" : "types")} and " +
                $"{unusedAssetPaths.Count} unused runtime proxy {(unusedAssetPaths.Count == 1 ? "asset" : "assets")}:\n\n" +
                $"Types: {unusedTypeList}\n" +
                $"Assets: {unusedAssetSummary}\n\n" +
                "These scripts and their assets can be cleaned up with one click at: 'Saneject/Runtime Proxy/Cleanup Unused Scripts And Assets'.\n" +
                "This warning can be disabled at: 'Saneject/Settings/User Settings/Log Unused Proxies On Domain Reload'.\n"
            );
        }

        public static void CleanUnusedScriptsAndAssets()
        {
            HashSet<Type> unusedTypes = GetUnusedTypes();
            HashSet<string> unusedAssetPaths = GetUnusedAssetPaths(unusedTypes);

            if (unusedTypes.Count == 0 && unusedAssetPaths.Count == 0)
            {
                DialogUtility.ProxyCleaner.Display_NoUnusedProxies();
                return;
            }

            FindAndDeleteAssets(unusedAssetPaths);
            FindAndDeleteScripts(unusedTypes);
        }

        private static HashSet<Type> GetUnusedTypes()
        {
            HashSet<Type> allTypes = TypeCache
                .GetTypesDerivedFrom<RuntimeProxyBase>()
                .Where(t => !t.IsAbstract)
                .ToHashSet();

            HashSet<Type> unusedTypes = allTypes
                .Where(t => t.BaseType is { IsGenericType: true })
                .Where(t => RuntimeProxyManifestUtility.EnumerateManifestTypes().All(mt => t.BaseType.GenericTypeArguments[0] != mt))
                .Where(t => t.GetCustomAttribute<MuteUnusedRuntimeProxyWarningAttribute>() == null)
                .ToHashSet();

            return unusedTypes;
        }

        private static HashSet<string> GetUnusedAssetPaths(HashSet<Type> unusedTypes)
        {
            HashSet<string> proxyAssetPaths = GetAllProxyAssetPaths();

            if (proxyAssetPaths.Count == 0)
                return proxyAssetPaths;

            HashSet<string> unreferencedProxyAssetPaths = proxyAssetPaths
                .Except(GetReferencedProxyAssetPaths(proxyAssetPaths))
                .ToHashSet();

            HashSet<string> unusedTypeAssetPaths = unusedTypes
                .SelectMany(t => AssetDatabase.FindAssets($"t:{t.Name}"))
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(proxyAssetPaths.Contains)
                .ToHashSet();

            unreferencedProxyAssetPaths.UnionWith(unusedTypeAssetPaths);
            return unreferencedProxyAssetPaths;
        }

        private static void FindAndDeleteAssets(HashSet<string> pathsToDelete)
        {
            if (pathsToDelete.Count == 0)
                return;

            HashSet<string> proxyTypeNames = pathsToDelete
                .Select(AssetDatabase.LoadAssetAtPath<RuntimeProxyBase>)
                .Where(asset => asset != null)
                .Select(asset => asset.GetType().Name)
                .ToHashSet();

            if (!DialogUtility.ProxyCleaner.Confirm_DeleteAssets(pathsToDelete.Count, proxyTypeNames))
                return;

            foreach (string path in pathsToDelete)
                AssetDatabase.DeleteAsset(path);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static HashSet<string> GetAllProxyAssetPaths()
        {
            HashSet<string> proxyAssetPaths = new();

            foreach (Type proxyType in TypeCache.GetTypesDerivedFrom<RuntimeProxyBase>().Where(t => !t.IsAbstract))
                foreach (string guid in AssetDatabase.FindAssets($"t:{proxyType.Name}"))
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);

                    if (string.IsNullOrWhiteSpace(path))
                        continue;

                    if (AssetDatabase.LoadAssetAtPath(path, proxyType) is RuntimeProxyBase)
                        proxyAssetPaths.Add(path);
                }

            return proxyAssetPaths;
        }

        private static HashSet<string> GetReferencedProxyAssetPaths(HashSet<string> proxyAssetPaths)
        {
            string[] projectAssetPaths = AssetDatabase
                .GetAllAssetPaths()
                .Where(path => path.StartsWith("Assets/", StringComparison.Ordinal))
                .Where(path => !path.EndsWith(".meta", StringComparison.OrdinalIgnoreCase))
                .Where(path => !AssetDatabase.IsValidFolder(path))
                .Where(path => !proxyAssetPaths.Contains(path))
                .ToArray();

            return AssetDatabase
                .GetDependencies(projectAssetPaths, true)
                .Where(proxyAssetPaths.Contains)
                .ToHashSet();
        }

        private static void FindAndDeleteScripts(HashSet<Type> unusedTypes)
        {
            string[] scriptGuids = AssetDatabase.FindAssets("t:MonoScript");
            List<string> pathsToDelete = new();

            foreach (string guid in scriptGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(path);

                if (script == null)
                    continue;

                Type scriptType = script.GetClass();

                if (scriptType == null)
                    continue;

                if (unusedTypes.Contains(scriptType))
                    pathsToDelete.Add(path);
            }

            if (pathsToDelete.Count == 0)
                return;

            if (!DialogUtility.ProxyCleaner.Confirm_DeleteScripts(unusedTypes.Select(t => t.Name)))
                return;

            foreach (string path in pathsToDelete)
                AssetDatabase.DeleteAsset(path);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}