using System.Collections.Generic;
using Plugins.Saneject.Experimental.Editor.Data;
using Plugins.Saneject.Experimental.Editor.Extensions;
using Plugins.Saneject.Experimental.Editor.Graph;
using Plugins.Saneject.Experimental.Editor.Utils;
using Plugins.Saneject.Runtime.Settings;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Editor.Core
{
    public static class Logger
    {
        public static void LogBindingErrors(IReadOnlyList<BindingError> errors)
        {
            foreach (BindingError error in errors)
                Debug.LogError(error.ErrorMessage, error.Transform);
        }

        public static void LogDependencyErrors(IReadOnlyList<DependencyError> dependencyErrors)
        {
        }

        public static void LogUnusedBindings(InjectionGraph graph)
        {
            if (!UserSettings.LogUnusedBindings)
                return;

            foreach (BindingContext context in graph.GetUnusedBindingContexts())
                Debug.LogWarning($"Saneject: Unused binding {BindingSignatureBuilder.GetBindingSignature(context)}. If you don't plan to use this binding, you can safely remove it.", context.Transform);
        }

        public static void LogStats(int elapsedMilliseconds)
        {
            Debug.Log($"Saneject: Injection took {elapsedMilliseconds}ms.");
        }
    }
}