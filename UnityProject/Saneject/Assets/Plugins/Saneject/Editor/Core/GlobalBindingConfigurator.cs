using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Plugins.Saneject.Editor.Extensions;
using Plugins.Saneject.Runtime.Bindings;
using Plugins.Saneject.Runtime.Extensions;
using Plugins.Saneject.Runtime.Global;
using Plugins.Saneject.Runtime.Scopes;
using Plugins.Saneject.Runtime.Settings;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Plugins.Saneject.Editor.Core
{
    public static class GlobalBindingConfigurator
    {
        /// <summary>
        /// Registers all scene-global bindings by creating a SceneGlobalContainer and populating it.
        /// </summary>
        public static void BuildSceneGlobalContainer(
            this Scope[] scopes,
            InjectionStats stats)
        {
            SceneGlobalContainer[] existingContainers = Object.FindObjectsByType<SceneGlobalContainer>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            foreach (SceneGlobalContainer container in existingContainers)
                container.DestroySelfOrGameObjectIfSolo();

            SceneGlobalContainer sceneGlobalContainer = null;

            foreach (Scope scope in scopes)
            {
                List<Binding> globalBindings = scope.GetGlobalBindings();

                if (globalBindings == null || globalBindings.Count == 0)
                    continue;

                if (scope.gameObject.IsPrefab())
                {
                    Debug.LogWarning($"Saneject: Global bindings found on prefab scope '{scope.GetType().Name}'. These are ignored because the system can only inject global bindings from scenes.", scope);
                    continue;
                }

                foreach (Binding globalBinding in globalBindings)
                {
                    Object resolved = globalBinding
                        .LocateDependencies()
                        .FirstOrDefault();

                    globalBinding.MarkUsed();

                    if (UserSettings.FilterBySameContext && resolved is Component component && component.gameObject.IsPrefab())
                    {
                        Debug.LogError($"Saneject: Global binding failed to locate a dependency {globalBinding.GetBindingSignature()}. Candidate rejected due to scene/prefab context mismatch: '{resolved.name}.{resolved.GetType().Name}'. Inject a non-prefab component, or disable filtering in User Settings (not recommended).");
                        stats.numMissingDependencies++;
                        continue;
                    }

                    if (!resolved)
                    {
                        Debug.LogError($"Saneject: Global binding failed to locate a dependency {globalBinding.GetBindingSignature()}.", scope);
                        stats.numMissingDependencies++;
                        continue;
                    }

                    if (!sceneGlobalContainer)
                    {
                        GameObject go = new(nameof(SceneGlobalContainer));
                        sceneGlobalContainer = go.AddComponent<SceneGlobalContainer>();

                        FieldInfo field = typeof(SceneGlobalContainer).GetField("createdByDependencyInjector", BindingFlags.Instance | BindingFlags.NonPublic);

                        if (field != null)
                            field.SetValue(sceneGlobalContainer, true);
                        else
                            Debug.LogError("Saneject: Could not find 'createdByDependencyInjector' field.", sceneGlobalContainer);

                        EditorSceneManager.MarkSceneDirty(go.scene);
                    }

                    if (sceneGlobalContainer.Add(resolved))
                    {
                        stats.numInjectedGlobal++;
                    }
                    else
                    {
                        Debug.LogError($"Saneject: Global binding dependency type '{globalBinding.ConcreteType}'' already added to SceneGlobalContainer {globalBinding.GetBindingSignature()}.", scope);
                        stats.numInvalidBindings++;
                    }
                }
            }
        }
    }
}