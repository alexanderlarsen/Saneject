using System.Collections.Generic;
using Plugins.Saneject.Experimental.Editor.Data;
using Plugins.Saneject.Experimental.Editor.Extensions;
using Plugins.Saneject.Experimental.Editor.Graph;
using Plugins.Saneject.Experimental.Editor.Graph.Extensions;
using Plugins.Saneject.Experimental.Editor.Graph.Nodes;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Editor.Core
{
    public static class BindingValidator
    {
        public static void ValidateBindings(
            InjectionGraph graph,
            out IReadOnlyList<BindingError> bindingErrors)
        {
            List<BindingError> allErrors = new();

            foreach (BindingNode binding in graph.EnumerateAllBindingNodes())
            {
                ValidateBinding(binding, out List<BindingError> errors);
                allErrors.AddRange(errors);
            }

            bindingErrors = allErrors;
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
                        errors.Add(new BindingError(binding, "A binding cannot be both Proxy and Global. Proxies consume globals; they are not globals themselves."));

                    if (binding.ScopeNode.TransformNode.ContextNode.IsPrefab)
                        errors.Add(new BindingError(binding, "Global bindings cannot be used in prefabs, because the system can only inject global components from scenes."));

                    if (globalBinding.IsCollectionBinding)
                        errors.Add(new BindingError(binding, "Global bindings must be singletons. Collections are not supported in the GlobalScope."));

                    if (globalBinding.IdQualifiers.Count > 0)
                        errors.Add(new BindingError(binding, "Global bindings cannot have IDs. The GlobalScope always resolves by type only."));

                    break;
                }

                case ComponentBindingNode componentBinding:
                {
                    if (componentBinding.ResolveFromProxy)
                    {
                        if (componentBinding.InterfaceType == null)
                            errors.Add(new BindingError(binding, "Proxy bindings require an interface type so the ProxyObject can forward calls. Use BindComponent<IInterface, Concrete>().FromProxy()."));

                        if (componentBinding.ConcreteType == null)
                            errors.Add(new BindingError(binding, "Proxy bindings require a concrete type to resolve into. Use BindComponent<IInterface, Concrete>().FromProxy()."));

                        if (componentBinding.IsCollectionBinding)
                            errors.Add(new BindingError(binding, "Proxy bindings must be single-value only. Collections cannot be resolved via a ProxyObject."));
                    }

                    if (componentBinding.ConcreteType != null && !typeof(Component).IsAssignableFrom(componentBinding.ConcreteType))
                        errors.Add(new BindingError(binding, $"Component binding type '{componentBinding.ConcreteType.Name}' is not a Unity Component. Component bindings must resolve UnityEngine.Component types."));

                    break;
                }

                case AssetBindingNode assetBinding:
                {
                    if (assetBinding.ConcreteType != null && typeof(Component).IsAssignableFrom(assetBinding.ConcreteType))
                        errors.Add(new BindingError(binding, $"Asset binding type '{assetBinding.ConcreteType.Name}' derives from Component. Assets must be ScriptableObjects, prefabs, or other UnityEngine.Object assets."));

                    break;
                }
            }

            if (binding.InterfaceType is { IsInterface: false })
                errors.Add(new BindingError(binding, $"Binding interface type '{binding.InterfaceType.FullName}' is not an interface."));

            if (binding.InterfaceType != null && binding.ConcreteType != null &&
                binding.InterfaceType.IsInterface &&
                !binding.InterfaceType.IsAssignableFrom(binding.ConcreteType))
                errors.Add(new BindingError(binding, $"Concrete type '{binding.ConcreteType.Name}' does not implement interface '{binding.InterfaceType.Name}'."));

            if (!binding.LocatorStrategySpecified)
                errors.Add(new BindingError(binding, "Binding has no locator strategy (e.g. FromScopeSelf, FromAnywhereInScene)."));

            binding.IsValid = errors.Count == 0;
        }
    }
}