using System;
using System.Collections.Generic;
using Plugins.Saneject.Experimental.Editor.Data;
using Plugins.Saneject.Experimental.Editor.Graph.Nodes;
using Plugins.Saneject.Runtime.Global;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Editor.Core
{
    public static class BindingValidator
    {
        public static void ValidateBindings(InjectionContext context)
        {
            Dictionary<Type, GlobalComponentBindingNode> existingGlobalMap = new();
            HashSet<BindingNode> existingBindings = new();

            foreach (BindingNode binding in context.Graph.EnumerateAllBindingNodes())
                ValidateBinding(binding, context, existingGlobalMap, existingBindings);
        }

        private static void ValidateBinding(
            BindingNode binding,
            InjectionContext context,
            Dictionary<Type, GlobalComponentBindingNode> existingGlobals,
            HashSet<BindingNode> existingBindings)
        {
            List<Error> errors = new();

            if (!existingBindings.Add(binding))
                errors.Add(Error.CreateInvalidBindingError("Duplicate binding within same Scope detected.", binding));

            switch (binding)
            {
                case GlobalComponentBindingNode globalBinding:
                {
                    if (globalBinding.ResolveFromProxy)
                        errors.Add(Error.CreateInvalidBindingError("A binding cannot be both Proxy and Global. Proxies consume globals; they are not globals themselves.", binding));

                    if (globalBinding.IsCollectionBinding)
                        errors.Add(Error.CreateInvalidBindingError("Global bindings must be singletons. Collections are not supported in the GlobalScope.", binding));

                    if (globalBinding.IdQualifiers.Count > 0)
                        errors.Add(Error.CreateInvalidBindingError("Global bindings cannot have IDs. The GlobalScope always resolves by type only.", binding));

                    if (existingGlobals.TryGetValue(globalBinding.ConcreteType, out GlobalComponentBindingNode existingGlobal))
                        errors.Add(Error.CreateInvalidBindingError($"Duplicate global binding '{globalBinding.ConcreteType.Name}' declared by '{globalBinding.ScopeNode.Type.Name}'. Already owned by '{existingGlobal.ScopeNode.Type.Name}'. Only one global per type is allowed.", binding));
                    else
                        existingGlobals.Add(globalBinding.ConcreteType, globalBinding);

                    break;
                }

                case ComponentBindingNode componentBinding:
                {
                    if (componentBinding.ResolveFromProxy)
                    {
                        if (componentBinding.InterfaceType == null)
                            errors.Add(Error.CreateInvalidBindingError("Proxy bindings require an interface type so the ProxyObject can forward calls. Use BindComponent<IInterface, Concrete>().FromProxy().", binding));

                        if (componentBinding.ConcreteType == null)
                            errors.Add(Error.CreateInvalidBindingError("Proxy bindings require a concrete type to resolve into. Use BindComponent<IInterface, Concrete>().FromProxy().", binding));

                        if (componentBinding.IsCollectionBinding)
                            errors.Add(Error.CreateInvalidBindingError("Proxy bindings must be single-value only. Collections cannot be resolved via a ProxyObject.", binding));
                    }

                    if (componentBinding.ConcreteType != null && !typeof(Component).IsAssignableFrom(componentBinding.ConcreteType))
                        errors.Add(Error.CreateInvalidBindingError($"Component binding type '{componentBinding.ConcreteType.Name}' is not a Unity Component. Component bindings must resolve UnityEngine.Component types.", binding));

                    break;
                }

                case AssetBindingNode assetBinding:
                {
                    if (assetBinding.ConcreteType != null && typeof(Component).IsAssignableFrom(assetBinding.ConcreteType))
                        errors.Add(Error.CreateInvalidBindingError($"Asset binding type '{assetBinding.ConcreteType.Name}' derives from Component. Assets must be ScriptableObjects, prefabs, or other UnityEngine.Object assets.", binding));

                    break;
                }
            }

            if (binding.InterfaceType is { IsInterface: false })
                errors.Add(Error.CreateInvalidBindingError($"Binding interface type '{binding.InterfaceType.FullName}' is not an interface.", binding));

            if (binding.InterfaceType != null && binding.ConcreteType != null &&
                binding.InterfaceType.IsInterface &&
                !binding.InterfaceType.IsAssignableFrom(binding.ConcreteType))
                errors.Add(Error.CreateInvalidBindingError($"Concrete type '{binding.ConcreteType.Name}' does not implement interface '{binding.InterfaceType.Name}'.", binding));

            if (!binding.LocatorStrategySpecified)
                errors.Add(Error.CreateInvalidBindingError("Binding has no locator strategy (e.g. FromScopeSelf, FromAnywhereInScene).", binding));

            context.RegisterErrors(errors);

            if (errors.Count == 0)
                context.RegisterValidBinding(binding);
        }
    }
}