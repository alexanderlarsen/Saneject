using Plugins.Saneject.Editor.Extensions;
using Plugins.Saneject.Runtime.Bindings;
using UnityEngine;

namespace Plugins.Saneject.Editor.Utility
{
    /// <summary>
    /// Validates user-declared <see cref="Binding" />s against Saneject’s rules.
    /// Reports invalid combinations and usage errors with detailed log messages.
    /// Validation goals:
    /// - Ensure bindings map to resolvable, concrete Unity types.
    /// - Prevent contradictory flags (e.g. Global + Collection).
    /// - Enforce correct usage of ProxyObject bindings.
    /// - Maintain determinism and prevent silent resolution failures.
    /// </summary>
    public static class BindingValidator
    {
        /// <summary>
        /// Returns whether a binding is valid. Logs descriptive error messages for all violations.
        /// </summary>
        public static bool IsBindingValid(Binding binding)
        {
            bool isValid = true;

            // Concrete must be instantiable
            if (binding.ConcreteType is { IsAbstract: true })
            {
                Debug.LogError(
                    $"Saneject: Invalid binding {binding.GetBindingIdentity()}." +
                    $" Concrete type '{binding.ConcreteType?.Name}' is abstract. Injected components must be non-abstract.",
                    binding.Scope);

                isValid = false;
            }

            if (binding.IsProxyBinding)
            {
                if (!binding.IsComponentBinding)
                {
                    Debug.LogError(
                        $"Saneject: Invalid binding {binding.GetBindingIdentity()}." +
                        " Proxy bindings are only supported for Component bindings since ProxyObjects wrap scene components.",
                        binding.Scope);

                    isValid = false;
                }

                if (binding.InterfaceType == null)
                {
                    Debug.LogError(
                        $"Saneject: Invalid binding {binding.GetBindingIdentity()}." +
                        " Proxy bindings require an interface type so the ProxyObject can forward calls. Use BindComponent<IInterface, Concrete>().FromProxy().",
                        binding.Scope);

                    isValid = false;
                }

                if (binding.ConcreteType == null)
                {
                    Debug.LogError(
                        $"Saneject: Invalid binding {binding.GetBindingIdentity()}." +
                        " Proxy bindings require a concrete type to resolve into. Use BindComponent<IInterface, Concrete>().FromProxy().",
                        binding.Scope);

                    isValid = false;
                }

                if (binding.IsCollection)
                {
                    Debug.LogError(
                        $"Saneject: Invalid binding {binding.GetBindingIdentity()}." +
                        " Proxy bindings must be single-value only. Collections cannot be resolved via a ProxyObject.",
                        binding.Scope);

                    isValid = false;
                }

                if (binding.IsGlobal)
                {
                    Debug.LogError(
                        $"Saneject: Invalid binding {binding.GetBindingIdentity()}." +
                        " A binding cannot be both Proxy and Global. Proxies consume globals; they are not globals themselves.",
                        binding.Scope);

                    isValid = false;
                }
            }

            if (binding.IsAssetBinding && binding.ConcreteType != null && typeof(Component).IsAssignableFrom(binding.ConcreteType))
            {
                Debug.LogError(
                    $"Saneject: Invalid binding {binding.GetBindingIdentity()}." +
                    $" Asset binding type '{binding.ConcreteType.Name}' derives from Component. Assets must be ScriptableObjects, prefabs, or other UnityEngine.Object assets.",
                    binding.Scope);

                isValid = false;
            }

            if (binding.IsComponentBinding && binding.ConcreteType != null && !typeof(Component).IsAssignableFrom(binding.ConcreteType))
            {
                Debug.LogError(
                    $"Saneject: Invalid binding {binding.GetBindingIdentity()}." +
                    $" Component binding type '{binding.ConcreteType.Name}' is not a Unity Component. Component bindings must resolve UnityEngine.Component types.",
                    binding.Scope);

                isValid = false;
            }

            if (binding.InterfaceType is { IsInterface: false })
            {
                Debug.LogError(
                    $"Saneject: Invalid binding {binding.GetBindingIdentity()}." +
                    $" Declared interface type '{binding.InterfaceType.FullName}' is not an interface.",
                    binding.Scope);

                isValid = false;
            }

            if (binding.InterfaceType != null && binding.ConcreteType != null &&
                binding.InterfaceType.IsInterface &&
                !binding.InterfaceType.IsAssignableFrom(binding.ConcreteType))
            {
                Debug.LogError(
                    $"Saneject: Invalid binding {binding.GetBindingIdentity()}." +
                    $" Concrete type '{binding.ConcreteType.Name}' does not implement interface '{binding.InterfaceType.Name}'.",
                    binding.Scope);

                isValid = false;
            }

            if (binding.IsGlobal && binding.Scope.gameObject.IsPrefab())
            {
                Debug.LogError(
                    $"Saneject: Invalid binding {binding.GetBindingIdentity()}." +
                    " Global bindings cannot be used in prefabs, because the system can only inject global components from scenes.",
                    binding.Scope);

                isValid = false;
            }

            if (binding.IsGlobal && binding.IsCollection)
            {
                Debug.LogError(
                    $"Saneject: Invalid binding {binding.GetBindingIdentity()}." +
                    " Global bindings must be singletons. Collections are not supported in the GlobalScope.",
                    binding.Scope);

                isValid = false;
            }

            if (binding.IsGlobal && !string.IsNullOrWhiteSpace(binding.Id))
            {
                Debug.LogError(
                    $"Saneject: Invalid binding {binding.GetBindingIdentity()}." +
                    " Global bindings cannot have IDs. The GlobalScope always resolves by type only.",
                    binding.Scope);

                isValid = false;
            }

            if (!binding.HasLocator)
            {
                Debug.LogError(
                    $"Saneject: Invalid binding {binding.GetBindingIdentity()}." +
                    " Binding has no locator strategy (e.g. FromScopeSelf, FromAnywhereInScene).",
                    binding.Scope);

                isValid = false;
            }

            return isValid;
        }
    }
}