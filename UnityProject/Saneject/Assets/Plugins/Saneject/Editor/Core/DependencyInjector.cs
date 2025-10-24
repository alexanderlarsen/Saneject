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

            // Application.isBatchMode is true when run from CI tests and CI test runner can't confirm the dialog so we need to skip it in that case.
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

            // Application.isBatchMode is true when run from CI tests and CI test runner can't confirm the dialog so we need to skip it in that case.
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
        /// Only used for prefab assets - does not register or inject global dependencies.
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

            // Application.isBatchMode is true when run from CI tests and CI test runner can't confirm the dialog so we need to skip it in that case.
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

                serializedProperty.Clear();

                bool isCollection = serializedProperty.isArray;
                Object unityContext = serializedObject.targetObject;
                Type injectionTargetType = serializedProperty.GetDeclaringType(serializedObject.targetObject);

                // Build a context-aware signature for field logging
                string injectedFieldSignature = FieldSignatureHelper.GetInjectedFieldSignature(serializedObject, serializedProperty, injectId);

                // Resolve via shared path
                if (!TryResolveDependency(
                        scope,
                        serializedObject,
                        interfaceType,
                        concreteType,
                        isCollection,
                        injectId,
                        memberName: serializedProperty.GetDisplayName(),
                        injectionTargetType: injectionTargetType,
                        injectionTarget: unityContext,
                        siteSignature: injectedFieldSignature,
                        stats: stats,
                        out Object proxyAsset,
                        out Object[] dependencies))
                    continue;

                // Apply resolution to the field
                if (proxyAsset != null)
                {
                    serializedProperty.objectReferenceValue = proxyAsset;
                    stats.numInjectedFields++;
                }
                else if (dependencies != null && dependencies.Length > 0)
                {
                    if (isCollection)
                        serializedProperty.SetCollection(dependencies);
                    else
                        serializedProperty.objectReferenceValue = dependencies.FirstOrDefault();

                    stats.numInjectedFields++;
                }
            }

            // Apply field injections first
            serializedObject.ApplyModifiedPropertiesWithoutUndo();

            // Now perform method injection AFTER all fields are serialized
            InjectMethods(serializedObject, scope, stats);
        }

        /// <summary>
        /// Handles method injection for [Inject] attributed methods.
        /// </summary>
        private static void InjectMethods(
            SerializedObject serializedObject,
            Scope scope,
            InjectionStats stats)
        {
            Object rootTarget = serializedObject.targetObject;
            Type rootType = rootTarget.GetType();

            // Inject methods on the root object
            InjectMethodsOnType(rootTarget, rootType, serializedObject, scope, stats);

            // Inject methods on nested serializable classes
            InjectMethodsInNestedSerializables(serializedObject, scope, stats);
        }

        /// <summary>
        /// Injects methods on nested [Serializable] classes within the serialized object.
        /// </summary>
        private static void InjectMethodsInNestedSerializables(
            SerializedObject serializedObject,
            Scope scope,
            InjectionStats stats)
        {
            SerializedProperty property = serializedObject.GetIterator();

            while (property.NextVisible(enterChildren: true))
                if (property.propertyType == SerializedPropertyType.Generic)
                {
                    object nestedObject = property.GetValue();

                    if (nestedObject != null && nestedObject.GetType().IsDefined(typeof(SerializableAttribute), false))
                    {
                        Type nestedType = nestedObject.GetType();
                        InjectMethodsOnType(nestedObject, nestedType, serializedObject, scope, stats);
                    }
                }
        }

        /// <summary>
        /// Injects methods on a specific object type.
        /// </summary>
        private static void InjectMethodsOnType(
            object injectionTarget,
            Type injectionTargetType,
            SerializedObject serializedObject,
            Scope scope,
            InjectionStats stats)
        {
            MethodInfo[] methods = injectionTargetType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (MethodInfo method in methods)
            {
                InjectAttribute injectAttribute = method.GetCustomAttribute<InjectAttribute>();

                if (injectAttribute == null)
                    continue;

                ParameterInfo[] parameters = method.GetParameters();
                object[] invokeParameters = new object[parameters.Length];
                bool allParametersResolved = true;

                for (int i = 0; i < parameters.Length; i++)
                {
                    ParameterInfo p = parameters[i];

                    bool isArray = p.ParameterType.IsArray;

                    bool isList = p.ParameterType.IsGenericType &&
                                  p.ParameterType.GetGenericTypeDefinition() == typeof(List<>);

                    bool isCollection = isArray || isList;

                    // Normalize to element type for binding lookup
                    Type elementType = isArray
                        ? p.ParameterType.GetElementType()
                        : isList
                            ? p.ParameterType.GetGenericArguments()[0]
                            : p.ParameterType;

                    if (elementType == null)
                        break;
                    
                    Type interfaceType = elementType.IsInterface ? elementType : null;
                    Type concreteType = elementType.IsInterface ? null : elementType;

                    // Build a context-aware signature for method logging
                    string siteSignature = MethodSignatureHelper.GetInjectedMethodSignature(method, injectionTarget, injectAttribute.ID);

                    // Resolve via shared path
                    if (!TryResolveDependency(
                            scope,
                            serializedObject,
                            interfaceType,
                            concreteType,
                            isCollection,
                            injectAttribute.ID,
                            memberName: p.Name,
                            injectionTargetType: injectionTargetType,
                            injectionTarget: serializedObject.targetObject,
                            siteSignature: siteSignature,
                            stats: stats,
                            out Object proxyAsset,
                            out Object[] dependencies))
                    {
                        allParametersResolved = false;
                        break;
                    }

                    if (proxyAsset != null)
                    {
                        // Assume the parameter type matches the proxy asset type when proxy binding is used
                        invokeParameters[i] = proxyAsset;
                        continue;
                    }

                    // Build a typed collection that matches the parameter signature
                    if (isCollection)
                        invokeParameters[i] = BuildTypedCollectionForMethodParam(p.ParameterType, elementType, dependencies);
                    else
                        invokeParameters[i] = dependencies.FirstOrDefault();
                }

                if (!allParametersResolved)
                    continue;

                // Update serialized object to get latest state
                serializedObject.Update();

                // Invoke the method
                method.Invoke(injectionTarget, invokeParameters);

                // Serialize the changes made by the method
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(serializedObject.targetObject);
                stats.numInjectedMethods++;
            }
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

        // -----------------------------
        // Shared resolution helpers
        // -----------------------------

        private static bool TryResolveDependency(
            Scope scope,
            SerializedObject serializedObject,
            Type interfaceType,
            Type concreteType,
            bool isCollection,
            string injectId,
            string memberName,
            Type injectionTargetType,
            Object injectionTarget,
            string siteSignature,
            InjectionStats stats,
            out Object proxyAsset,
            out Object[] dependencies)
        {
            proxyAsset = null;
            dependencies = null;

            Binding binding = scope.GetBindingRecursiveUpwards(
                interfaceType,
                concreteType,
                injectId,
                isCollection,
                memberName,
                injectionTargetType);

            if (binding == null)
            {
                // Context-aware, keep existing partial binding formatting
                string partialBindingSignature = BindingSignatureHelper.GetPartialBindingSignature(isCollection, interfaceType, concreteType, scope);
                Debug.LogError($"Saneject: Missing binding {partialBindingSignature} {siteSignature}", scope);

                stats.numMissingBindings++;
                return false;
            }

            binding.MarkUsed();

            // Proxy resolution path
            if (binding.IsProxyBinding)
            {
                Type proxyType = ProxyUtils.GetProxyTypeFromConcreteType(binding.ConcreteType);

                if (proxyType != null)
                {
                    proxyAsset = ProxyUtils.GetFirstOrCreateProxyAsset(proxyType, out _);
                    return true;
                }

                Debug.LogError($"Saneject: Missing ProxyObject<{binding.ConcreteType.Name}> for binding {binding.GetBindingSignature()} {siteSignature}", scope);
                stats.numMissingDependencies++;
                return false;
            }

            // Locate candidates
            Object[] found = binding.LocateDependencies(injectionTarget).ToArray();

            HashSet<Type> rejectedTypes = null;

            if (UserSettings.FilterBySameContext)
                found = found.FilterBySameContext(serializedObject, out rejectedTypes);

            if (found.Length > 0)
            {
                dependencies = found;
                return true;
            }

            // Context-aware failure with detailed rejections, mirroring field path format
            StringBuilder msg = new();

            msg.AppendLine($"Saneject: Binding failed to locate a dependency {binding.GetBindingSignature()} {siteSignature}.");

            if (rejectedTypes is { Count: > 0 })
            {
                string typeList = string.Join(", ", rejectedTypes.Select(t => t.Name));
                msg.AppendLine($"Candidates rejected due to scene/prefab or prefab/prefab context mismatch: {typeList}.");
                msg.AppendLine("Use ProxyObjects for proper cross-context references, or disable filtering in User Settings (not recommended).");
            }

            Debug.LogError(msg.ToString(), scope);
            stats.numMissingDependencies++;
            return false;
        }

        private static object BuildTypedCollectionForMethodParam(
            Type parameterType,
            Type elementType,
            Object[] dependencies)
        {
            if (parameterType.IsArray)
            {
                Array typedArray = Array.CreateInstance(elementType, dependencies.Length);

                for (int i = 0; i < dependencies.Length; i++)
                    typedArray.SetValue(dependencies[i], i);

                return typedArray;
            }

            if (parameterType.IsGenericType && parameterType.GetGenericTypeDefinition() == typeof(List<>))
            {
                Type listType = typeof(List<>).MakeGenericType(elementType);
                object list = Activator.CreateInstance(listType);
                MethodInfo add = listType.GetMethod("Add");

                for (int i = 0; i < dependencies.Length; i++)
                    add.Invoke(list, new object[] { dependencies[i] });

                return list;
            }

            // Fallback - should not happen for supported collection types, return array
            Array fallback = Array.CreateInstance(elementType, dependencies.Length);

            for (int i2 = 0; i2 < dependencies.Length; i2++)
                fallback.SetValue(dependencies[i2], i2);

            return fallback;
        }
    }
}