using System.Collections.Generic;
using Plugins.Saneject.Experimental.Editor.Data;
using Plugins.Saneject.Experimental.Editor.Graph;
using Plugins.Saneject.Experimental.Editor.Graph.Nodes;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Editor.Core
{
    public static class BindingConfigValidator
    {
        public static void ValidateBindings(
            InjectionGraph graph,
            List<Error> errors)
        {
            foreach (BindingNode binding in graph.EnumerateAllBindingNodes())
            {
                ValidateBinding(binding, out List<Error> validationErrors);
                errors.AddRange(validationErrors);
            }
        }

        private static void ValidateBinding(
            BindingNode binding,
            out List<Error> validationErrors)
        {
            validationErrors = new List<Error>();

            switch (binding)
            {
                case GlobalComponentBindingNode globalBinding:
                {
                    if (globalBinding.ResolveFromProxy)
                        validationErrors.Add(Error.CreateInvalidBindingError("A binding cannot be both Proxy and Global. Proxies consume globals; they are not globals themselves.", binding));

                    if (binding.ScopeNode.TransformNode.ContextNode.IsPrefab)
                        validationErrors.Add(Error.CreateInvalidBindingError("Global bindings cannot be used in prefabs, because the system can only inject global components from scenes.", binding));

                    if (globalBinding.IsCollectionBinding)
                        validationErrors.Add(Error.CreateInvalidBindingError("Global bindings must be singletons. Collections are not supported in the GlobalScope.", binding));

                    if (globalBinding.IdQualifiers.Count > 0)
                        validationErrors.Add(Error.CreateInvalidBindingError("Global bindings cannot have IDs. The GlobalScope always resolves by type only.", binding));

                    break;
                }

                case ComponentBindingNode componentBinding:
                {
                    if (componentBinding.ResolveFromProxy)
                    {
                        if (componentBinding.InterfaceType == null)
                            validationErrors.Add(Error.CreateInvalidBindingError("Proxy bindings require an interface type so the ProxyObject can forward calls. Use BindComponent<IInterface, Concrete>().FromProxy().", binding));

                        if (componentBinding.ConcreteType == null)
                            validationErrors.Add(Error.CreateInvalidBindingError("Proxy bindings require a concrete type to resolve into. Use BindComponent<IInterface, Concrete>().FromProxy().", binding));

                        if (componentBinding.IsCollectionBinding)
                            validationErrors.Add(Error.CreateInvalidBindingError("Proxy bindings must be single-value only. Collections cannot be resolved via a ProxyObject.", binding));
                    }

                    if (componentBinding.ConcreteType != null && !typeof(Component).IsAssignableFrom(componentBinding.ConcreteType))
                        validationErrors.Add(Error.CreateInvalidBindingError($"Component binding type '{componentBinding.ConcreteType.Name}' is not a Unity Component. Component bindings must resolve UnityEngine.Component types.", binding));

                    break;
                }

                case AssetBindingNode assetBinding:
                {
                    if (assetBinding.ConcreteType != null && typeof(Component).IsAssignableFrom(assetBinding.ConcreteType))
                        validationErrors.Add(Error.CreateInvalidBindingError($"Asset binding type '{assetBinding.ConcreteType.Name}' derives from Component. Assets must be ScriptableObjects, prefabs, or other UnityEngine.Object assets.", binding));

                    break;
                }
            }

            if (binding.InterfaceType is { IsInterface: false })
                validationErrors.Add(Error.CreateInvalidBindingError($"Binding interface type '{binding.InterfaceType.FullName}' is not an interface.", binding));

            if (binding.InterfaceType != null && binding.ConcreteType != null &&
                binding.InterfaceType.IsInterface &&
                !binding.InterfaceType.IsAssignableFrom(binding.ConcreteType))
                validationErrors.Add(Error.CreateInvalidBindingError($"Concrete type '{binding.ConcreteType.Name}' does not implement interface '{binding.InterfaceType.Name}'.", binding));

            if (!binding.LocatorStrategySpecified)
                validationErrors.Add(Error.CreateInvalidBindingError("Binding has no locator strategy (e.g. FromScopeSelf, FromAnywhereInScene).", binding));

            binding.IsValid = validationErrors.Count == 0;
        }
    }
}