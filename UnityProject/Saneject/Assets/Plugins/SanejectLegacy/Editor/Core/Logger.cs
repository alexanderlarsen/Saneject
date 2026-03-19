using System.Collections.Generic;
using Plugins.SanejectLegacy.Runtime.Bindings;
using Plugins.SanejectLegacy.Runtime.Scopes;
using Plugins.SanejectLegacy.Runtime.Settings;
using UnityEngine;

namespace Plugins.SanejectLegacy.Editor.Core
{
    public static class Logger
    {
        /// <summary>
        /// Logs warnings about any bindings that were configured but not actually used during injection.
        /// </summary>
        public static void LogUnusedBindings(
            IEnumerable<Scope> allScopes,
            InjectionStats stats)
        {
            foreach (Scope scope in allScopes)
            {
                List<Binding> unusedBindings = scope.GetUnusedBindings();

                if (UserSettings.LogUnusedBindings && unusedBindings.Count > 0)
                    foreach (Binding binding in unusedBindings)
                        Debug.LogWarning($"Saneject: Unused binding {binding.GetBindingSignature()}. If you don't plan to use this binding, you can safely remove it.", scope);

                stats.numUnusedBindings += unusedBindings.Count;
            }
        }
    }
}