using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Plugins.Saneject.Editor.Utilities;
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

            if (unusedTypes.Count == 0)
                return;

            int count = unusedTypes.Count;

            Debug.LogWarning
            (
                $"Saneject: Found {count} unused runtime proxy {(count == 1 ? "type" : "types")}:\n" +
                $"{string.Join(", ", unusedTypes.Select(t => t.Name))}\n\n" +
                "These scripts and their assets can be cleaned up with one click at: <i>Saneject → Runtime Proxy → Cleanup Unused Scripts And Assets</i>.\n" +
                "This warning can be disabled at: <i>Saneject → Settings → User Settings → Log Unused Proxies On Domain Reload</i>.\n"
            );
        }

        public static void CleanUnusedScriptsAndAssets()
        {
            HashSet<Type> unusedTypes = GetUnusedTypes();

            if (unusedTypes.Count == 0)
            {
                DialogUtility.ProxyCleaner.Display_NoUnusedProxies();
                return;
            }

            FindAndDeleteAssets(unusedTypes);
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
                .ToHashSet();

            return unusedTypes;
        }

        private static void FindAndDeleteAssets(HashSet<Type> unusedTypes)
        {
            HashSet<string> pathsToDelete = unusedTypes
                .SelectMany(t => AssetDatabase.FindAssets($"t:{t.Name}"))
                .Select(AssetDatabase.GUIDToAssetPath)
                .ToHashSet();

            if (pathsToDelete.Count == 0)
                return;

            if (!DialogUtility.ProxyCleaner.Confirm_DeleteAssets(pathsToDelete.Count, unusedTypes.Select(t => t.Name)))
                return;

            foreach (string path in pathsToDelete)
                AssetDatabase.DeleteAsset(path);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
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