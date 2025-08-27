using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Plugins.Saneject.Editor.Extensions;
using Plugins.Saneject.Editor.Util;
using Plugins.Saneject.Editor.Utility;
using Plugins.Saneject.Runtime.Attributes;
using Plugins.Saneject.Runtime.Bindings;
using Plugins.Saneject.Runtime.Extensions;
using Plugins.Saneject.Runtime.Global;
using Plugins.Saneject.Runtime.Scopes;
using Plugins.Saneject.Runtime.Settings;
using Saneject.Plugins.Saneject.Editor.Utility;
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
                EditorUtility.DisplayDialog("Saneject: Inject Scene Dependencies", "Injection is editor-only. Exit Play Mode to inject.", "Got it");
                return;
            }

            // Application.isBatchMode is true when run from CI tests.
            if (!Application.isBatchMode)
            {
                if (UserSettings.AskBeforeSceneInjection && !EditorUtility.DisplayDialog("Saneject: Inject Scene Dependencies", "Are you sure you want to inject all dependencies in the scene?", "Yes", "Cancel"))
                    return;

                if (UserSettings.ClearLogsOnInjection)
                    ConsoleUtils.ClearLog();
            }

            Stopwatch stopwatch = Stopwatch.StartNew();

            Scope[] allScopes = Object
                .FindObjectsByType<Scope>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .Where(scope => !scope.gameObject.IsPrefab())
                .ToArray();

            if (allScopes.Length <= 0)
            {
                Debug.LogWarning("Saneject: No scopes found in scene. Nothing to inject.");
                stopwatch.Stop();
                return;
            }

            try
            {
                EditorUtility.DisplayProgressBar("Saneject: Injection in progress", "Injecting all objects in scene", 0);

                ScopeExtensions.Initialize(allScopes);

                Scope[] rootScopes = allScopes
                    .Where(scope => !scope.ParentScope)
                    .ToArray();

                IEnumerable<Binding> proxyBindings = allScopes
                    .SelectMany(scope => scope.GetProxyBindings());

                proxyBindings.CreateMissingProxyStubs(out bool isProxyCreationPending);

                if (isProxyCreationPending)
                {
                    stopwatch.Stop();
                    allScopes.Dispose();
                    EditorUtility.ClearProgressBar();
                    Debug.LogWarning("Saneject: Injection aborted due to proxy script creation.");
                    return;
                }

                InjectionStats stats = new();

                allScopes.ConfigureGlobalBindings(stats);

                foreach (Scope root in rootScopes)
                    root.InjectFromRoot(stats, isPrefabInjection: false);

                allScopes.LogUnusedBindings(stats);
                stopwatch.Stop();

                if (UserSettings.LogInjectionStats)
                {
                    stats.numInvalidBindings = allScopes.Sum(scope => scope.InvalidBindingsCount);
                    stats.LogStats("Full scene", allScopes.Length, stopwatch.ElapsedMilliseconds);
                }
            }
            catch (Exception e)
            {
                stopwatch.Stop();
                Debug.LogException(e);
            }

            allScopes.Dispose();
            EditorUtility.ClearProgressBar();
        }

        public static void InjectSingleHierarchyDependencies(Scope startScope)
        {
            if (Application.isPlaying)
            {
                EditorUtility.DisplayDialog("Saneject: Inject Hierarchy Dependencies", "Injection is editor-only. Exit Play Mode to inject.", "Got it");
                return;
            }

            // Application.isBatchMode is true when run from CI tests.
            if (!Application.isBatchMode)
            {
                if (UserSettings.AskBeforeHierarchyInjection && !EditorUtility.DisplayDialog("Saneject: Inject Hierarchy Dependencies", "Are you sure you want to inject all dependencies in the hierarchy?", "Yes", "Cancel"))
                    return;

                if (UserSettings.ClearLogsOnInjection)
                    ConsoleUtils.ClearLog();
            }

            Stopwatch stopwatch = Stopwatch.StartNew();
            Scope rootScope = startScope.FindRootScope();

            Scope[] allScopes = rootScope
                .GetComponentsInChildren<Scope>()
                .Where(scope => !scope.gameObject.IsPrefab())
                .ToArray();

            if (allScopes.Length <= 0)
            {
                Debug.LogWarning("Saneject: No scopes found in hierarchy. Nothing to inject.");
                stopwatch.Stop();
                return;
            }

            ScopeExtensions.Initialize(allScopes);

            try
            {
                EditorUtility.DisplayProgressBar("Saneject: Injection in progress", "Injecting all objects in hierarchy", 0);

                IEnumerable<Binding> proxyBindings = allScopes
                    .SelectMany(scope => scope.GetProxyBindings());

                proxyBindings.CreateMissingProxyStubs(out bool isProxyCreationPending);

                if (isProxyCreationPending)
                {
                    stopwatch.Stop();
                    allScopes.Dispose();
                    EditorUtility.ClearProgressBar();
                    Debug.LogWarning("Saneject: Injection aborted due to proxy script creation.");
                    return;
                }

                InjectionStats stats = new();

                allScopes.ConfigureGlobalBindings(stats);
                rootScope.InjectFromRoot(stats, isPrefabInjection: false);
                allScopes.LogUnusedBindings(stats);
                stopwatch.Stop();

                if (UserSettings.LogInjectionStats)
                {
                    stats.numInvalidBindings = allScopes.Sum(scope => scope.InvalidBindingsCount);
                    stats.LogStats($"Single scene hierarchy [{rootScope.gameObject.name}]", allScopes.Length, stopwatch.ElapsedMilliseconds);
                }
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
                EditorUtility.DisplayDialog("Saneject: Inject Prefab Dependencies", "Injection is editor-only. Exit Play Mode to inject.", "Got it");
                return;
            }

            // Application.isBatchMode is true when run from CI tests.
            if (!Application.isBatchMode)
            {
                if (UserSettings.AskBeforePrefabInjection && !EditorUtility.DisplayDialog("Saneject: Inject Prefab Dependencies", "Are you sure you want to inject all dependencies in the prefab?", "Yes", "Cancel"))
                    return;

                if (UserSettings.ClearLogsOnInjection)
                    ConsoleUtils.ClearLog();
            }

            Stopwatch stopwatch = Stopwatch.StartNew();
            Scope rootScope = startScope.FindRootScope();
            Scope[] allScopes = rootScope.GetComponentsInChildren<Scope>();

            if (allScopes.Length <= 0)
            {
                Debug.LogWarning("Saneject: No scopes found in prefab. Nothing to inject.");
                return;
            }

            ScopeExtensions.Initialize(allScopes);

            IEnumerable<Binding> proxyBindings = allScopes.SelectMany(scope => scope.GetProxyBindings());
            proxyBindings.CreateMissingProxyStubs(out bool isProxyCreationPending);

            if (isProxyCreationPending)
            {
                stopwatch.Stop();
                allScopes.Dispose();
                EditorUtility.ClearProgressBar();
                Debug.LogWarning("Saneject: Injection aborted due to proxy script creation.");
                return;
            }

            InjectionStats stats = new();
            EditorUtility.DisplayProgressBar("Saneject: Injection in progress", "Injecting prefab dependencies", 0);

            try
            {
                rootScope.InjectFromRoot(stats, isPrefabInjection: true);
                allScopes.LogUnusedBindings(stats);
                stopwatch.Stop();

                if (UserSettings.LogInjectionStats)
                {
                    stats.numInvalidBindings = allScopes.Sum(scope => scope.InvalidBindingsCount);
                    stats.LogStats($"Prefab [{rootScope.gameObject.name}]", allScopes.Length, stopwatch.ElapsedMilliseconds);
                }
            }
            catch (Exception e)
            {
                stopwatch.Stop();
                Debug.LogError($"Saneject Exception: {e}");
            }

            allScopes.Dispose();
            EditorUtility.ClearProgressBar();
        }

        private static void CreateMissingProxyStubs(
            this IEnumerable<Binding> proxyBindings,
            out bool isProxyCreationPending)
        {
            isProxyCreationPending = false;

            List<Type> typesToCreate = proxyBindings
                .Select(binding => binding.ConcreteType)
                .Where(type => !ProxyUtils.DoesProxyScriptExist(type)).ToList();

            if (typesToCreate.Count == 0)
                return;

            isProxyCreationPending = true;

            string scriptsWord = typesToCreate.Count == 1 ? "script" : "scripts";

            EditorUtility.DisplayDialog($"Saneject: Proxy {scriptsWord} required", $"{typesToCreate.Count} proxy {scriptsWord} will be created. Afterwards Unity will recompile and stop the current injection pass. Click 'Inject' again after recompilation to complete the injection.", "Got it");

            typesToCreate.ForEach(ProxyUtils.GenerateProxyScript);
            SessionState.SetInt("Saneject.ProxyStubCount", typesToCreate.Count);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
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
                        Debug.LogWarning($"Saneject: Unused binding {binding.GetBindingSignature()}. If you don't plan to use this binding, you can safely remove it.", scope);

                stats.numUnusedBindings += unusedBindings.Count;
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
                    Debug.LogWarning($"Saneject: Global bindings found on prefab scope '{scope.GetType().Name}'. These are ignored because the system can only inject global bindings from scenes.", scope);
                    continue;
                }

                foreach (Binding globalBinding in globalBindings)
                {
                    Object resolved = globalBinding
                        .LocateDependencies()
                        .FirstOrDefault();

                    if (!resolved)
                    {
                        Debug.LogError($"Saneject: Could not resolve global dependency in scope '{scope.gameObject.name}'", scope);
                        continue;
                    }

                    globalBinding.MarkUsed();

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

                    sceneGlobalContainer.Add(resolved);
                    stats.numInjectedGlobal++;
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
                    serializedObject.InjectSerializedObject(currentScope, stats);
                    EditorUtility.SetDirty(monoBehaviour);
                }

                foreach (Transform child in currentTransform)
                    InjectRecursive(child, currentScope);
            }
        }

        /// <summary>
        /// Handles assignment of references to serialized fields/properties on MonoBehaviours, including nested Serializable classes.
        /// </summary>
        private static void InjectSerializedObject(
            this SerializedObject serializedObject,
            Scope scope,
            InjectionStats stats)
        {
            SerializedProperty serializedProperty = serializedObject.GetIterator();

            while (serializedProperty.NextVisible(enterChildren: true))
            {
                if (!serializedProperty.IsInjectable(out Type interfaceType, out Type concreteType, out string injectId))
                    continue;

                serializedProperty.NullifyOrClearArray();

                bool isCollection = serializedProperty.isArray;
                Object injectionTarget = serializedObject.targetObject;
                Binding binding = scope.GetBindingRecursiveUpwards(interfaceType, concreteType, injectId, isCollection, injectionTarget, serializedProperty.GetDisplayName());
                string injectedFieldSignature = $"[Injected field path: {NamePathUtils.GetInjectedFieldPath(serializedObject, serializedProperty)}]";

                if (binding == null)
                {
                    Debug.LogError($"Saneject: Missing binding {BindingSignatureHelper.GetPartialBindingSignature(isCollection, interfaceType, concreteType, injectId, scope)} {injectedFieldSignature}", scope);
                    stats.numMissingBindings++;
                    continue;
                }

                binding.MarkUsed();

                if (binding.IsProxyBinding)
                {
                    Type proxyType = ProxyUtils.GetProxyTypeFromConcreteType(binding.ConcreteType);

                    if (proxyType != null)
                    {
                        serializedProperty.objectReferenceValue = ProxyUtils.GetFirstOrCreateProxyAsset(proxyType, out bool _);
                        stats.numInjectedFields++;
                    }
                    else
                    {
                        Debug.LogError($"Saneject: Missing ProxyObject<{binding.ConcreteType.Name}> for binding {binding.GetBindingSignature()} {injectedFieldSignature}", scope);
                        stats.numMissingDependencies++;
                    }

                    continue;
                }

                Object[] dependencies = binding.LocateDependencies(injectionTarget).ToArray();

                HashSet<Type> rejectedTypes = null;

                if (UserSettings.FilterBySameContext)
                    dependencies = dependencies.FilterBySameContext(serializedObject, out rejectedTypes);

                if (dependencies.Length > 0)
                {
                    if (isCollection)
                        serializedProperty.SetCollection(dependencies);
                    else
                        serializedProperty.objectReferenceValue = dependencies.FirstOrDefault();

                    stats.numInjectedFields++;
                    continue;
                }

                StringBuilder errorMessageBuilder = new();

                errorMessageBuilder.AppendLine(
                    $"Saneject: Binding failed to locate a dependency {binding.GetBindingSignature()} {injectedFieldSignature}.");

                if (rejectedTypes is { Count: > 0 })
                {
                    string typeList = string.Join(", ", rejectedTypes.Select(t => t.Name));

                    errorMessageBuilder.AppendLine(
                        $"Candidates rejected due to scene/prefab or prefab/prefab context mismatch: {typeList}.");

                    errorMessageBuilder.AppendLine(
                        "Use ProxyObjects for proper cross-context references, or disable filtering in User Settings (not recommended).");
                }

                stats.numMissingDependencies++;
                Debug.LogError(errorMessageBuilder.ToString(), scope);
            }

            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        /// <summary>
        /// Ensures that only dependencies from the same context as the injection target are kept.
        /// Scene objects only match within the same scene, prefab instances only within their root,
        /// and prefab assets only within their asset root. ScriptableObjects and other assets are unaffected.
        /// </summary>
        private static Object[] FilterBySameContext(
            this Object[] objects,
            SerializedObject serializedObject,
            out HashSet<Type> rejectedTypes)
        {
            rejectedTypes = null;

            if (objects.Length == 0)
                return objects;

            object targetContext = GetContextKey(serializedObject.targetObject);
            List<Object> filtered = new();

            foreach (Object obj in objects)
            {
                if (obj is Component comp)
                {
                    object candidateContext = GetContextKey(comp);

                    // Reject if they don't share the same context
                    if (!Equals(targetContext, candidateContext))
                    {
                        rejectedTypes ??= new HashSet<Type>();
                        rejectedTypes.Add(comp.GetType());
                        continue;
                    }
                }

                // Non-Component objects (ScriptableObjects, etc.) are always valid
                filtered.Add(obj);
            }

            return filtered.ToArray();
        }

        /// <summary>
        /// Returns a context "key" that uniquely identifies whether an object belongs
        /// to a scene, a prefab instance, or a prefab asset. Prefab assets and their
        /// instances normalize to the same prefab asset root so they are treated as
        /// one context.
        /// </summary>
        private static object GetContextKey(Object obj)
        {
            if (obj is not Component component)
                return null; // unrestricted (ScriptableObjects, etc.)

            GameObject go = component.gameObject;

            // 1) Prefab INSTANCE
            if (PrefabUtility.IsPartOfPrefabInstance(go))
            {
                GameObject instanceRoot = PrefabUtility.GetOutermostPrefabInstanceRoot(go);

                if (instanceRoot)
                {
                    GameObject prefabAsset = PrefabUtility.GetCorrespondingObjectFromSource(instanceRoot);
                    return prefabAsset ? (Object)prefabAsset : instanceRoot;
                }

                return go.scene; // fallback
            }

            // 2) Prefab ASSET in Project window
            if (PrefabUtility.IsPartOfPrefabAsset(go))
                // The root of the prefab asset is just its transform.root
                return go.transform.root.gameObject;

            // 3) Open Prefab Stage
            PrefabStage stage = PrefabStageUtility.GetPrefabStage(go);

            if (stage && stage.prefabContentsRoot)
            {
                GameObject asset = PrefabUtility.GetCorrespondingObjectFromSource(stage.prefabContentsRoot);
                return asset ? (Object)asset : stage.prefabContentsRoot;
            }

            // 4) Scene object
            return go.scene;
        }

        /// <summary>
        /// Returns true if the given field/property is eligible for injection and returns its metadata.
        /// </summary>
        private static bool IsInjectable(
            this SerializedProperty serializedProperty,
            out Type interfaceType,
            out Type concreteType,
            out string injectId)
        {
            interfaceType = null;
            concreteType = null;
            injectId = null;

            FieldInfo field = serializedProperty.GetFieldInfo();

            if (field == null)
                return false;

            if (!field.IsDefined(typeof(SerializeField)) && !field.IsPublic)
                return false;

            concreteType = serializedProperty.isArray ? field.FieldType.GetElementType() : field.FieldType;

            InterfaceBackingFieldAttribute interfaceBackingFieldAttribute = field.GetCustomAttribute<InterfaceBackingFieldAttribute>(true);

            if (interfaceBackingFieldAttribute is { IsInjected: true })
            {
                interfaceType = interfaceBackingFieldAttribute.InterfaceType;
                concreteType = null;
                injectId = interfaceBackingFieldAttribute.InjectId;
                return true;
            }

            InjectAttribute injectAttribute = field.GetCustomAttribute<InjectAttribute>(true);

            if (injectAttribute != null && typeof(object).IsAssignableFrom(field.FieldType))
            {
                injectId = injectAttribute.ID;
                return true;
            }

            return false;
        }
    }
}