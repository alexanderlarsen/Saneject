using System.Collections.Generic;
using Plugins.Saneject.Experimental.Editor.Data;
using Plugins.Saneject.Experimental.Editor.Graph;
using Plugins.Saneject.Experimental.Editor.Graph.Extensions;
using Plugins.Saneject.Experimental.Editor.Graph.Nodes;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Editor.Core
{
    public static class BindingConfigValidator
    {
        public static void ValidateBindings(
            InjectionGraph graph,
            out List<BindingError> bindingErrors)
        {
            bindingErrors = new List<BindingError>();

            foreach (BindingNode binding in graph.EnumerateAllBindingNodes())
            {
                ValidateBinding(binding, out List<BindingError> errors);
                bindingErrors.AddRange(errors);
            }
        }

        private static void ValidateBinding(
            BindingNode binding,
            out List<BindingError> errors)
        {
            errors = new List<BindingError>();

            switch (binding)
            {
                case GlobalComponentBindingNode globalBinding:
                {
                    if (globalBinding.ResolveFromProxy)
                        errors.Add(BindingError.CreateInvalidConfigError("A binding cannot be both Proxy and Global. Proxies consume globals; they are not globals themselves.", binding));

                    if (binding.ScopeNode.TransformNode.ContextNode.IsPrefab)
                        errors.Add(BindingError.CreateInvalidConfigError("Global bindings cannot be used in prefabs, because the system can only inject global components from scenes.", binding));

                    if (globalBinding.IsCollectionBinding)
                        errors.Add(BindingError.CreateInvalidConfigError("Global bindings must be singletons. Collections are not supported in the GlobalScope.", binding));

                    if (globalBinding.IdQualifiers.Count > 0)
                        errors.Add(BindingError.CreateInvalidConfigError("Global bindings cannot have IDs. The GlobalScope always resolves by type only.", binding));

                    break;
                }

                case ComponentBindingNode componentBinding:
                {
                    if (componentBinding.ResolveFromProxy)
                    {
                        if (componentBinding.InterfaceType == null)
                            errors.Add(BindingError.CreateInvalidConfigError("Proxy bindings require an interface type so the ProxyObject can forward calls. Use BindComponent<IInterface, Concrete>().FromProxy().", binding));

                        if (componentBinding.ConcreteType == null)
                            errors.Add(BindingError.CreateInvalidConfigError("Proxy bindings require a concrete type to resolve into. Use BindComponent<IInterface, Concrete>().FromProxy().", binding));

                        if (componentBinding.IsCollectionBinding)
                            errors.Add(BindingError.CreateInvalidConfigError("Proxy bindings must be single-value only. Collections cannot be resolved via a ProxyObject.", binding));
                    }

                    if (componentBinding.ConcreteType != null && !typeof(Component).IsAssignableFrom(componentBinding.ConcreteType))
                        errors.Add(BindingError.CreateInvalidConfigError($"Component binding type '{componentBinding.ConcreteType.Name}' is not a Unity Component. Component bindings must resolve UnityEngine.Component types.", binding));

                    break;
                }

                case AssetBindingNode assetBinding:
                {
                    if (assetBinding.ConcreteType != null && typeof(Component).IsAssignableFrom(assetBinding.ConcreteType))
                        errors.Add(BindingError.CreateInvalidConfigError($"Asset binding type '{assetBinding.ConcreteType.Name}' derives from Component. Assets must be ScriptableObjects, prefabs, or other UnityEngine.Object assets.", binding));

                    break;
                }
            }

            if (binding.InterfaceType is { IsInterface: false })
                errors.Add(BindingError.CreateInvalidConfigError($"Binding interface type '{binding.InterfaceType.FullName}' is not an interface.", binding));

            if (binding.InterfaceType != null && binding.ConcreteType != null &&
                binding.InterfaceType.IsInterface &&
                !binding.InterfaceType.IsAssignableFrom(binding.ConcreteType))
                errors.Add(BindingError.CreateInvalidConfigError($"Concrete type '{binding.ConcreteType.Name}' does not implement interface '{binding.InterfaceType.Name}'.", binding));

            if (!binding.LocatorStrategySpecified)
                errors.Add(BindingError.CreateInvalidConfigError("Binding has no locator strategy (e.g. FromScopeSelf, FromAnywhereInScene).", binding));

            binding.IsValid = errors.Count == 0;
        }
    }
}