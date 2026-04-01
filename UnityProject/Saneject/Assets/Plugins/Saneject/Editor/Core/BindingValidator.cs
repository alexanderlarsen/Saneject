using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Plugins.Saneject.Editor.Data.Errors;
using Plugins.Saneject.Editor.Data.Graph.Nodes;
using Plugins.Saneject.Editor.Data.Injection;
using Plugins.Saneject.Editor.Utilities;
using UnityEditor;
using Component = UnityEngine.Component;

namespace Plugins.Saneject.Editor.Core
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
            BindingNode bindingNode,
            InjectionContext context,
            Dictionary<Type, GlobalComponentBindingNode> existingGlobals,
            HashSet<BindingNode> existingBindings)
        {
            List<InjectionError> errors = new();

            if (!existingBindings.Add(bindingNode))
                errors.Add(new InvalidBindingError
                (
                    bindingNode: bindingNode,
                    reason: "Duplicate binding within same Scope detected"
                ));

            switch (bindingNode)
            {
                case GlobalComponentBindingNode globalBinding:
                {
                    if (existingGlobals.TryGetValue(globalBinding.ConcreteType, out GlobalComponentBindingNode existingGlobal))
                        errors.Add(new InvalidBindingError
                        (
                            bindingNode: bindingNode,
                            reason: $"Duplicate global binding '{globalBinding.ConcreteType.Name}' declared by '{globalBinding.ScopeNode.ScopeType.Name}'. Already owned by '{existingGlobal.ScopeNode.ScopeType.Name}'. Only one global per type is allowed."
                        ));
                    else
                        existingGlobals.Add(globalBinding.ConcreteType, globalBinding);

                    break;
                }

                case ComponentBindingNode componentBinding:
                {
                    if (componentBinding.RuntimeProxyConfig != null)
                    {
                        if (componentBinding.InterfaceType == null)
                            errors.Add(new InvalidBindingError
                            (
                                bindingNode: bindingNode,
                                reason: "RuntimeProxy bindings require an interface type. Use BindComponent<IInterface, Concrete>().FromProxy()."
                            ));

                        if (componentBinding.ConcreteType == null)
                            errors.Add(new InvalidBindingError
                            (
                                bindingNode: bindingNode,
                                reason: "RuntimeProxy bindings require an interface type. Use BindComponent<IInterface, Concrete>().FromProxy()."
                            ));

                        if (componentBinding.IsCollectionBinding)
                            errors.Add(new InvalidBindingError
                            (
                                bindingNode: bindingNode,
                                reason: "RuntimeProxy bindings must be single-value only. Collections cannot be resolved via a RuntimeProxy."
                            ));
                    }

                    if (componentBinding.ConcreteType != null && !typeof(Component).IsAssignableFrom(componentBinding.ConcreteType))
                        errors.Add(new InvalidBindingError
                        (
                            bindingNode: bindingNode,
                            reason: $"Component binding type '{componentBinding.ConcreteType.Name}' is not a Unity Component. Component bindings must resolve UnityEngine.Component types."
                        ));

                    break;
                }

                case AssetBindingNode assetBinding:
                {
                    if (assetBinding.ConcreteType != null && typeof(Component).IsAssignableFrom(assetBinding.ConcreteType))
                        errors.Add(new InvalidBindingError
                        (
                            bindingNode: bindingNode,
                            reason: $"Asset binding type '{assetBinding.ConcreteType.Name}' derives from Component. Assets must be ScriptableObjects, prefabs, or other UnityEngine.Object assets."
                        ));
                    else if (assetBinding.ResolveFromInstances != null && assetBinding.ResolveFromInstances.Any(x => !EditorUtility.IsPersistent(x)))
                        errors.Add(new InvalidBindingError
                        (
                            bindingNode: bindingNode,
                            reason: "Asset binding configured with non-asset objects."
                        ));

                    break;
                }
            }

            if (bindingNode.FromMethodException != null)
                errors.Add(new InvalidBindingError
                (
                    bindingNode: bindingNode,
                    reason: "FromMethod(...) threw an exception.",
                    exception: bindingNode.FromMethodException
                ));

            if (bindingNode.InterfaceType is { IsInterface: false })
                errors.Add(new InvalidBindingError
                (
                    bindingNode: bindingNode,
                    reason: $"Binding interface type '{bindingNode.InterfaceType.FullName}' is not an interface."
                ));

            if (bindingNode.InterfaceType != null && bindingNode.ConcreteType != null &&
                bindingNode.InterfaceType.IsInterface &&
                !bindingNode.InterfaceType.IsAssignableFrom(bindingNode.ConcreteType))
                errors.Add(new InvalidBindingError
                (
                    bindingNode: bindingNode,
                    reason: $"Concrete type '{bindingNode.ConcreteType.Name}' does not implement interface '{bindingNode.InterfaceType.Name}'."
                ));

            if (!bindingNode.LocatorStrategySpecified)
                errors.Add(new InvalidBindingError
                (
                    bindingNode: bindingNode,
                    reason: "Binding has no locator strategy (e.g. FromScopeSelf, FromAnywhere)."
                ));

            context.RegisterErrors(errors);

            if (errors.Count == 0)
                context.RegisterValidBinding(bindingNode);
        }
    }
}