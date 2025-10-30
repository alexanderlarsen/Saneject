using System;
using System.Linq;
using System.Reflection;
using Plugins.Saneject.Editor.Extensions;
using Plugins.Saneject.Editor.Utility;
using Plugins.Saneject.Runtime.Attributes;
using Plugins.Saneject.Runtime.Scopes;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Editor.Core
{
    public static class PropertyInjector
    {
        public static void InjectSerializedProperties(
            SerializedObject serializedObject,
            Scope scope,
            InjectionStats stats)
        {
            SerializedProperty serializedProperty = serializedObject.GetIterator();

            while (serializedProperty.NextVisible(enterChildren: true))
            {
                if (!serializedProperty.IsPropertyInjectable(out Type interfaceType, out Type concreteType, out string injectId, out bool suppressMissingErrors))
                    continue;

                serializedProperty.Clear();

                bool isCollection = serializedProperty.isArray;
                Object unityContext = serializedObject.targetObject;
                Type injectionTargetType = serializedProperty.GetDeclaringType(serializedObject.targetObject);

                // Build a context-aware signature for field logging
                string injectedFieldSignature = FieldSignatureHelper.GetInjectedFieldSignature(serializedObject, serializedProperty, injectId);

                // Resolve via shared path
                if (!DependencyResolver.TryResolveDependency(
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
                        suppressMissingErrors: suppressMissingErrors,
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
                else if (dependencies is { Length: > 0 })
                {
                    if (isCollection)
                        serializedProperty.SetCollection(dependencies);
                    else
                        serializedProperty.objectReferenceValue = dependencies.FirstOrDefault();

                    stats.numInjectedFields++;
                }
            }
        }

        /// <summary>
        /// Returns true if the given field/property is eligible for injection and returns its metadata.
        /// </summary>
        private static bool IsPropertyInjectable(
            this SerializedProperty serializedProperty,
            out Type interfaceType,
            out Type concreteType,
            out string injectId,
            out bool suppressMissingErrors)
        {
            interfaceType = null;
            concreteType = null;
            injectId = null;
            suppressMissingErrors = false;

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
                suppressMissingErrors = injectAttribute.SuppressMissingErrors;
                injectId = injectAttribute.ID;
                return true;
            }

            return false;
        }
    }
}