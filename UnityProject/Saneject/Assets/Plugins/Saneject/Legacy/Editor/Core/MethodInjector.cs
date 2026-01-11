using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Plugins.Saneject.Legacy.Editor.Extensions;
using Plugins.Saneject.Legacy.Editor.Utility;
using Plugins.Saneject.Legacy.Runtime.Attributes;
using Plugins.Saneject.Legacy.Runtime.Scopes;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Legacy.Editor.Core
{
    public static class MethodInjector
    {
        /// <summary>
        /// Handles method injection for [Inject] attributed methods.
        /// </summary>
        public static void InjectMethods(
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
                    if (!DependencyResolver.TryResolveDependency(
                            scope,
                            serializedObject,
                            interfaceType,
                            concreteType,
                            isCollection,
                            injectAttribute.ID,
                            memberName: method.Name,
                            injectionTargetType: injectionTargetType,
                            injectionTarget: serializedObject.targetObject,
                            siteSignature: siteSignature,
                            suppressMissingErrors: injectAttribute.SuppressMissingErrors,
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
                        invokeParameters[i] = BuildTypedCollectionForMethodParameter(p.ParameterType, elementType, dependencies);
                    else
                        invokeParameters[i] = dependencies.FirstOrDefault();
                }

                if (!allParametersResolved)
                    continue;

                // Update serialized object to get latest state
                serializedObject.Update();

                // Invoke the method
                method.Invoke(injectionTarget, invokeParameters);

                stats.numInjectedMethods++;
            }
        }

        private static object BuildTypedCollectionForMethodParameter(
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
                MethodInfo addMethod = listType.GetMethod("Add");

                foreach (Object t in dependencies)
                    addMethod?.Invoke(list, new object[]
                    {
                        t
                    });

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