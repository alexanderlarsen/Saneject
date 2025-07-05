using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Plugins.Saneject.Editor.Extensions;
using Plugins.Saneject.Runtime.Attributes;
using Plugins.Saneject.Runtime.Bindings;
using Plugins.Saneject.Runtime.Extensions;
using Plugins.Saneject.Runtime.Global;
using Plugins.Saneject.Runtime.Scopes;
using Plugins.Saneject.Runtime.Settings;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Editor.Core
{
    /// <summary>
    /// Provides editor-only dependency injection for Saneject-enabled scenes and prefabs.
    /// Handles resolving and assigning dependencies for all <see cref="Scope" />s in the current scene or prefab,
    /// based on the binding strategies and filters configured by the user.
    /// </summary>
    public static class DependencyInjector
    {
        /// <summary>
        /// Performs dependency injection for all <see cref="Scope" />s in the currently open scene.
        /// Scans for all configured bindings, resolves them, and assigns references to all eligible fields/properties.
        /// This operation is editor-only and cannot be performed in Play Mode.
        /// </summary>
        public static void InjectSceneDependencies()
        {
            if (Application.isPlaying)
            {
                EditorUtility.DisplayDialog("Inject Scene Dependencies", "Injection is editor-only. Exit Play Mode to inject.", "Got it");
                return;
            }

            if (UserSettings.AskBeforeSceneInjection && !EditorUtility.DisplayDialog("Inject Scene Dependencies", "Are you sure you want to inject all dependencies in the scene?", "Yes", "Cancel"))
                return;

            Stopwatch stopwatch = Stopwatch.StartNew();
            Scope[] allScopes = Object.FindObjectsByType<Scope>(FindObjectsInactive.Include, FindObjectsSortMode.None).Where(scope => !scope.gameObject.IsPrefab()).ToArray();

            if (allScopes.Length <= 0)
            {
                Debug.LogWarning("Saneject: No scopes found in scene. Nothing to inject.");
                stopwatch.Stop();
                return;
            }

            try
            {
                EditorUtility.DisplayProgressBar("Injection in progress", "Injecting all objects in scene", 0);

                allScopes.Configure();
                Scope[] rootScopes = allScopes.Where(scope => !scope.ParentScope).ToArray();

                InjectionStats stats = new();

                allScopes.ConfigureGlobalBindings(stats);

                foreach (Scope root in rootScopes)
                    root.InjectFromRoot(stats, isPrefabInjection: false);

                allScopes.LogUnusedBindings(stats);
                stopwatch.Stop();

                if (UserSettings.LogInjectionStats)
                    Debug.Log($"Saneject: Scene injection complete | {allScopes.Length} scopes processed | {stats.injectedGlobal} global dependencies | {stats.injectedFields} injected fields | {stats.missingBindings} missing bindings | {stats.unusedBindings} unused bindings | Completed in {stopwatch.ElapsedMilliseconds} ms");
            }
            catch (Exception e)
            {
                stopwatch.Stop();
                Debug.LogException(e);
            }

            allScopes.Dispose();
            EditorUtility.ClearProgressBar();
        }

        /// <summary>
        /// Performs dependency injection for all <see cref="Scope" />s under a prefab root.
        /// Only used for prefab assets; does not register or inject global dependencies.
        /// This operation is editor-only and cannot be performed in Play Mode.
        /// </summary>
        /// <param name="startScope">The root scope in the prefab to start injection from.</param>
        public static void InjectPrefabDependencies(Scope startScope)
        {
            if (!startScope)
            {
                Debug.LogWarning("Saneject: No scopes found in prefab. Nothing to inject.");
                return;
            }

            if (Application.isPlaying)
            {
                EditorUtility.DisplayDialog("Inject Prefab Dependencies", "Injection is editor-only. Exit Play Mode to inject.", "Got it");
                return;
            }

            if (UserSettings.AskBeforePrefabInjection && !EditorUtility.DisplayDialog("Inject Prefab Dependencies", "Are you sure you want to inject all dependencies in the prefab?", "Yes", "Cancel"))
                return;

            Scope rootScope = startScope.FindRootScope();
            Scope[] allScopes = rootScope.GetComponentsInChildren<Scope>();

            if (allScopes.Length <= 0)
            {
                Debug.LogWarning("Saneject: No scopes found in prefab. Nothing to inject.");
                return;
            }
            
            allScopes.Configure();

            InjectionStats stats = new();

            foreach (Scope scope in allScopes)
            {
                List<Binding> globalBindings = scope.GetGlobalBindings();

                if (globalBindings.Count > 0)
                    foreach (Binding binding in globalBindings)
                        Debug.LogWarning($"Saneject: Invalid global binding '{binding.concreteType.Name}' in prefab scope '{scope.GetType().Name}'. Global bindings on prefab scopes are ignored because the system can only inject global bindings from scenes.", scope);

                stats.unusedBindings += globalBindings.Count;
            }

            EditorUtility.DisplayProgressBar("Injection in progress", "Injecting prefab dependencies", 0);
            Stopwatch stopwatch = Stopwatch.StartNew();

            try
            {
                rootScope.InjectFromRoot(stats, isPrefabInjection: true);
                allScopes.LogUnusedBindings(stats);
                stopwatch.Stop();

                if (UserSettings.LogInjectionStats)
                    Debug.Log($"Saneject: Prefab injection complete | {allScopes.Length} scopes processed | {stats.injectedFields} injected fields | {stats.missingBindings} missing bindings | {stats.unusedBindings} unused bindings | Completed in {stopwatch.ElapsedMilliseconds} ms");
            }
            catch (Exception e)
            {
                stopwatch.Stop();
                Debug.LogError($"Saneject Exception: {e}");
            }

            allScopes.Dispose();
            EditorUtility.ClearProgressBar();
        }

        /// <summary>
        /// Logs warnings about any bindings that were configured but not actually used during injection.
        /// </summary>
        private static void LogUnusedBindings(
            this IEnumerable<Scope> allScopes,
            InjectionStats stats)
        {
            foreach (Scope scope in allScopes)
            {
                List<Binding> unusedBindings = scope.GetUnusedBindings();

                if (UserSettings.LogUnusedBindings && unusedBindings.Count > 0)
                    foreach (Binding binding in unusedBindings)
                        Debug.LogWarning(
                            binding.interfaceType != null
                                ? $"Saneject: Unused binding '{binding.interfaceType.Name}/{binding.concreteType.Name}' in scope '{scope.name}'. If you don't plan to use this binding, you can safely remove it."
                                : $"Saneject: Unused binding '{binding.concreteType.Name}' in scope '{scope.name}'. If you don't plan to use this binding, you can safely remove it.", scope);

                stats.unusedBindings += unusedBindings.Count;
            }
        }

        /// <summary>
        /// Registers all scene-global bindings by creating a SceneGlobalContainer and populating it.
        /// </summary>
        private static void ConfigureGlobalBindings(
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
                    Debug.LogWarning($"Saneject: Global bindings found on prefab scope '{scope.gameObject.name}'. These are ignored because the system can only inject global bindings from scenes.", scope.gameObject);
                    continue;
                }

                foreach (Binding globalBinding in globalBindings)
                {
                    Object resolved = globalBinding.LocateDependency();

                    if (!resolved)
                    {
                        Debug.LogError($"Saneject: Could not resolve global dependency in scope {scope.gameObject.name}");
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
                            Debug.LogError("Saneject: Could not find 'createdByDependencyInjector' field.");

                        EditorSceneManager.MarkSceneDirty(go.scene);
                    }

                    sceneGlobalContainer.Add(resolved);
                    stats.injectedGlobal++;
                }
            }
        }

        /// <summary>
        /// Performs recursive injection for all child objects in a hierarchy rooted at a given Scope.
        /// </summary>
        private static void InjectFromRoot(
            this Scope rootScope,
            InjectionStats stats,
            bool isPrefabInjection)
        {
            if (!isPrefabInjection && rootScope.gameObject.IsPrefab())
            {
                if (UserSettings.LogPrefabSkippedDuringSceneInjection)
                    Debug.LogWarning($"Saneject: Skipping scene injection on prefab scope '{rootScope.GetType().Name}' on '{rootScope.gameObject.name}'. The prefab must solve its dependencies using its own scope.", rootScope.gameObject);

                return;
            }

            Dictionary<CacheKey, Object> cache = new();
            InjectRecursive(rootScope.transform, rootScope);
            return;

            void InjectRecursive(
                Transform currentTransform,
                Scope currentScope)
            {
                if (!isPrefabInjection && currentTransform.gameObject.IsPrefab())
                {
                    if (UserSettings.LogPrefabSkippedDuringSceneInjection)
                        Debug.LogWarning($"Saneject: Skipping scene injection on prefab '{currentTransform.gameObject.name}'. The prefab must solve its dependencies using its own scope", currentTransform.gameObject);

                    return;
                }

                if (currentTransform.TryGetComponent(out Scope localScope) && localScope != currentScope)
                    currentScope = localScope;

                foreach (MonoBehaviour monoBehaviour in currentTransform.GetComponents<MonoBehaviour>())
                {
                    SerializedObject serializedObject = new(monoBehaviour);
                    InjectSerializedObject(serializedObject, currentScope, cache, stats);
                    EditorUtility.SetDirty(monoBehaviour);
                }

                foreach (Transform child in currentTransform)
                    InjectRecursive(child, currentScope);
            }
        }

        /// <summary>
        /// Handles assignment of references to serialized fields/properties on MonoBehaviours.
        /// </summary>
        private static void InjectSerializedObject(
            SerializedObject serializedObject,
            Scope scope,
            Dictionary<CacheKey, Object> cache,
            InjectionStats stats)
        {
            SerializedProperty serializedProperty = serializedObject.GetIterator();

            while (serializedProperty.NextVisible(enterChildren: true))
                if (serializedProperty.propertyType == SerializedPropertyType.ObjectReference)
                    if (serializedProperty.IsEligibleForInjection(serializedObject.targetObject.GetType(), out string propertyInjectId, out Type targetType))
                    {
                        Object resolved = ResolveDependency(scope, targetType, propertyInjectId, cache, serializedObject.targetObject);

                        if (resolved)
                        {
                            serializedProperty.objectReferenceValue = resolved;
                            stats.injectedFields++;
                        }
                        else
                        {
                            serializedProperty.objectReferenceValue = null;
                            string idString = !string.IsNullOrEmpty(propertyInjectId) ? $"(ID: {propertyInjectId}) " : string.Empty;
                            Debug.LogError($"Saneject: Missing binding '{targetType.Name}' {idString}in scope '{scope.name}'", scope);
                            stats.missingBindings++;
                        }
                    }

            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        /// <summary>
        /// Returns true if the given field/property is eligible for injection and returns its metadata.
        /// </summary>
        private static bool IsEligibleForInjection(
            this SerializedProperty serializedProperty,
            Type rootType,
            out string propertyInjectId,
            out Type targetType)
        {
            propertyInjectId = null;
            targetType = null;

            FieldInfo field = GetFieldFromProperty(rootType, serializedProperty.propertyPath);

            if (field == null)
                return false;

            if (field.GetCustomAttribute<SerializeField>() == null)
                return false;

            InterfaceBackingFieldAttribute interfaceAttr = field.GetCustomAttribute<InterfaceBackingFieldAttribute>(true);

            if (interfaceAttr is { IsInjected: true })
            {
                targetType = interfaceAttr.InterfaceType;
                propertyInjectId = interfaceAttr.InjectId;
                return true;
            }

            InjectAttribute injectAttr = field.GetCustomAttribute<InjectAttribute>(true);

            if (injectAttr != null && typeof(Object).IsAssignableFrom(field.FieldType))
            {
                targetType = field.FieldType;
                propertyInjectId = injectAttr.ID;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns the <see cref="FieldInfo" /> for a field with the given name, searching the entire class hierarchy (base types included).
        /// </summary>
        private static FieldInfo GetFieldInClassHierarchy(
            Type type,
            string fieldName)
        {
            while (type != null && type != typeof(object))
            {
                FieldInfo field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                if (field != null)
                    return field;

                type = type.BaseType;
            }

            return null;
        }

        /// <summary>
        /// Navigates property path and returns the corresponding <see cref="FieldInfo" /> (if present).
        /// </summary>
        private static FieldInfo GetFieldFromProperty(
            Type type,
            string propertyPath)
        {
            string[] parts = propertyPath.Split('.');
            Type currentType = type;
            FieldInfo field = null;

            foreach (string part in parts)
            {
                field = GetFieldInClassHierarchy(currentType, part);

                if (field == null)
                    return null;

                currentType = field.FieldType;
            }

            return field;
        }

        /// <summary>
        /// Resolves a dependency via scope binding, with caching for faster processing.
        /// </summary>
        private static Object ResolveDependency(
            Scope scope,
            Type targetType,
            string id,
            Dictionary<CacheKey, Object> cache,
            Object injectionTarget)
        {
            Binding binding = scope.GetBindingRecursiveUpwards(id, targetType);

            if (binding == null)
                return null;

            // For bindings that require target, include injectionTarget in the key
            CacheKey key = new(id, targetType, scope, binding.RequiresInjectionTarget ? injectionTarget : null);

            if (cache.TryGetValue(key, out Object cached))
                return cached;

            Object resolved = binding.LocateDependency(injectionTarget);

            if (resolved != null)
                cache[key] = resolved;

            return resolved;
        }

        /// <summary>
        /// Class for collecting injection stats for logging purposes.
        /// </summary>
        private class InjectionStats
        {
            public int injectedGlobal;
            public int injectedFields;
            public int missingBindings;
            public int unusedBindings;
        }

        /// <summary>
        /// Cache key for ensuring uniqueness in the resolved dependency cache.
        /// </summary>
        private class CacheKey : IEquatable<CacheKey>
        {
            private readonly string id;
            private readonly Type type;
            private readonly Scope scope;
            private readonly Object injectionTarget;

            public CacheKey(
                string id,
                Type type,
                Scope scope,
                Object injectionTarget = null)
            {
                this.id = id;
                this.type = type;
                this.scope = scope;
                this.injectionTarget = injectionTarget;
            }

            public bool Equals(CacheKey other)
            {
                return other != null
                       && Equals(id, other.id)
                       && type == other.type
                       && scope == other.scope
                       && Equals(injectionTarget, other.injectionTarget);
            }

            public override bool Equals(object obj)
            {
                return obj is CacheKey other && Equals(other);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(id, type, scope, injectionTarget);
            }
        }
    }
}