using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Plugins.Saneject.Editor.Util;
using Plugins.Saneject.Runtime.Bindings;
using Plugins.Saneject.Runtime.Scopes;
using Plugins.Saneject.Runtime.Settings;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Editor.Core
{
    public static class DependencyResolver
    {
        public static bool TryResolveDependency(
            Scope scope,
            SerializedObject serializedObject,
            Type interfaceType,
            Type concreteType,
            bool isCollection,
            string injectId,
            string memberName,
            Type injectionTargetType,
            Object injectionTarget,
            string siteSignature,
            bool suppressMissingErrors,
            InjectionStats stats,
            out Object proxyAsset,
            out Object[] dependencies)
        {
            proxyAsset = null;
            dependencies = null;

            Binding binding = scope.GetBindingRecursiveUpwards(
                interfaceType,
                concreteType,
                injectId,
                isCollection,
                memberName,
                injectionTargetType);

            if (binding == null)
            {
                if (suppressMissingErrors)
                {
                    stats.numSuppressedMissing++;
                }
                else
                {
                    // Context-aware, keep existing partial binding formatting
                    string partialBindingSignature = BindingSignatureHelper.GetPartialBindingSignature(isCollection, interfaceType, concreteType, scope);
                    Debug.LogError($"Saneject: Missing binding {partialBindingSignature} {siteSignature}", scope);
                    stats.numMissingBindings++;
                }

                return false;
            }

            binding.MarkUsed();

            // Proxy resolution path
            if (binding.IsProxyBinding)
            {
                Type proxyType = ProxyUtils.GetProxyTypeFromConcreteType(binding.ConcreteType);

                if (proxyType != null)
                {
                    proxyAsset = ProxyUtils.GetFirstOrCreateProxyAsset(proxyType, out _);
                    return true;
                }

                Debug.LogError($"Saneject: Missing ProxyObject<{binding.ConcreteType.Name}> for binding {binding.GetBindingSignature()} {siteSignature}", scope);
                stats.numMissingDependencies++;
                return false;
            }

            // Locate candidates
            Object[] found = binding.LocateDependencies(injectionTarget).ToArray();

            HashSet<Type> rejectedTypes = null;

            if (UserSettings.FilterBySameContext)
                found = ContextFilter.FilterBySameContext(found, serializedObject, out rejectedTypes);

            if (found.Length > 0)
            {
                dependencies = found;
                return true;
            }

            // Suppress error messages for optional dependencies
            if (suppressMissingErrors)
            {
                stats.numSuppressedMissing++;
                return false;
            }

            // Context-aware failure with detailed rejections, mirroring field path format
            StringBuilder msg = new();

            msg.AppendLine($"Saneject: Binding failed to locate a dependency {binding.GetBindingSignature()} {siteSignature}.");

            if (rejectedTypes is { Count: > 0 })
            {
                string typeList = string.Join(", ", rejectedTypes.Select(t => t.Name));
                msg.AppendLine($"Candidates rejected due to scene/prefab or prefab/prefab context mismatch: {typeList}.");
                msg.AppendLine("Use ProxyObjects for proper cross-context references, or disable filtering in User Settings (not recommended).");
            }

            Debug.LogError(msg.ToString(), scope);
            stats.numMissingDependencies++;
            return false;
        }
    }
}