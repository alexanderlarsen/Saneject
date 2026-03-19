using System;
using System.Collections.Generic;
using System.ComponentModel;
using Plugins.Saneject.Editor.Data.Graph.Nodes;
using Plugins.Saneject.Editor.Data.Injection;
using Plugins.Saneject.Editor.Data.Logging;
using Plugins.Saneject.Editor.Utilities;
using Component = UnityEngine.Component;

namespace Plugins.Saneject.Editor.Pipeline
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class BindingValidator
    {
        public static void ValidateBindings(
            InjectionContext context,
            InjectionProgressTracker progressTracker)
        {
            progressTracker.BeginSegment(stepCount: context.ActiveBindingNodes.Count);

            Dictionary<Type, GlobalComponentBindingNode> existingGlobalMap = new();
            HashSet<BindingNode> existingBindings = new();

            foreach (BindingNode binding in context.ActiveBindingNodes)
            {
                progressTracker.UpdateInfoText($"Validating binding: {SignatureUtility.GetBindingSignature(binding)}");
                ValidateBinding(binding, context, existingGlobalMap, existingBindings);
                progressTracker.NextStep();
            }
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
                    if (existingGlobals.TryGetValue(globalBinding.ConcreteType, out GlobalComponentBindingNode existingGlobal))
                        errors.Add(Error.CreateInvalidBindingError($"Duplicate global binding '{globalBinding.ConcreteType.Name}' declared by '{globalBinding.ScopeNode.ScopeType.Name}'. Already owned by '{existingGlobal.ScopeNode.ScopeType.Name}'. Only one global per type is allowed.", binding));
                    else
                        existingGlobals.Add(globalBinding.ConcreteType, globalBinding);

                    break;
                }

                case ComponentBindingNode componentBinding:
                {
                    if (componentBinding.RuntimeProxyConfig != null)
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
                errors.Add(Error.CreateInvalidBindingError("Binding has no locator strategy (e.g. FromScopeSelf, FromAnywhere).", binding));

            context.RegisterErrors(errors);

            if (errors.Count == 0)
                context.RegisterValidBinding(binding);
        }
    }
}