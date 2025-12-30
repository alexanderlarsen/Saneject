using System.Collections.Generic;
using Plugins.Saneject.Experimental.Editor.Data;
using Plugins.Saneject.Experimental.Editor.Extensions;
using Plugins.Saneject.Experimental.Editor.Graph;
using Plugins.Saneject.Experimental.Editor.Graph.BindingNodes;
using Plugins.Saneject.Runtime.Extensions;
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

            foreach (BindingContext binding in graph.GetAllBindingContexts())
            {
                ValidateBinding(binding, out List<BindingError> errors);
                allErrors.AddRange(errors);
            }

            bindingErrors = allErrors;
        }

        private static void ValidateBinding(
            BindingContext context,
            out List<BindingError> errors)
        {
            errors = new List<BindingError>();
            BaseBindingNode binding = context.Binding;

            switch (binding)
            {
                case GlobalComponentBindingNode globalBinding:
                {
                    if (globalBinding.ResolveFromProxy)
                        errors.Add(new BindingError(context, "A binding cannot be both Proxy and Global. Proxies consume globals; they are not globals themselves."));

                    if (context.Transform.gameObject.IsPrefab())
                        errors.Add(new BindingError(context, "Global bindings cannot be used in prefabs, because the system can only inject global components from scenes."));

                    if (globalBinding.IsCollectionBinding)
                        errors.Add(new BindingError(context, "Global bindings must be singletons. Collections are not supported in the GlobalScope."));

                    if (globalBinding.IdQualifiers.Count > 0)
                        errors.Add(new BindingError(context, "Global bindings cannot have IDs. The GlobalScope always resolves by type only."));

                    break;
                }

                case ComponentBindingNode componentBinding:
                {
                    if (componentBinding.ResolveFromProxy)
                    {
                        if (componentBinding.InterfaceType == null)
                            errors.Add(new BindingError(context, "Proxy bindings require an interface type so the ProxyObject can forward calls. Use BindComponent<IInterface, Concrete>().FromProxy()."));

                        if (componentBinding.ConcreteType == null)
                            errors.Add(new BindingError(context, "Proxy bindings require a concrete type to resolve into. Use BindComponent<IInterface, Concrete>().FromProxy()."));

                        if (componentBinding.IsCollectionBinding)
                            errors.Add(new BindingError(context, "Proxy bindings must be single-value only. Collections cannot be resolved via a ProxyObject."));
                    }

                    if (componentBinding.ConcreteType != null && !typeof(Component).IsAssignableFrom(componentBinding.ConcreteType))
                        errors.Add(new BindingError(context, $"Component binding type '{componentBinding.ConcreteType.Name}' is not a Unity Component. Component bindings must resolve UnityEngine.Component types."));

                    break;
                }

                case AssetBindingNode assetBinding:
                {
                    if (assetBinding.ConcreteType != null && typeof(Component).IsAssignableFrom(assetBinding.ConcreteType))
                        errors.Add(new BindingError(context, $"Asset binding type '{assetBinding.ConcreteType.Name}' derives from Component. Assets must be ScriptableObjects, prefabs, or other UnityEngine.Object assets."));

                    break;
                }
            }

            if (binding.InterfaceType is { IsInterface: false })
                errors.Add(new BindingError(context, $"Binding interface type '{binding.InterfaceType.FullName}' is not an interface."));

            if (binding.InterfaceType != null && binding.ConcreteType != null &&
                binding.InterfaceType.IsInterface &&
                !binding.InterfaceType.IsAssignableFrom(binding.ConcreteType))
                errors.Add(new BindingError(context, $"Concrete type '{binding.ConcreteType.Name}' does not implement interface '{binding.InterfaceType.Name}'."));

            if (!binding.LocatorStrategySpecified)
                errors.Add(new BindingError(context, "Binding has no locator strategy (e.g. FromScopeSelf, FromAnywhereInScene)."));

            context.Binding.IsValid = errors.Count == 0;
        }
    }
}