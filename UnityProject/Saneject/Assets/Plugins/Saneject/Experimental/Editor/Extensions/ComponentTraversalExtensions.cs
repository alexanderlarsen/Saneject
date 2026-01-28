using System;
using System.Collections.Generic;
using System.Reflection;
using Plugins.Saneject.Experimental.Editor.Data.TraversalResults;
using Plugins.Saneject.Experimental.Runtime.Attributes;
using UnityEngine;

// ReSharper disable LoopCanBePartlyConvertedToQuery

namespace Plugins.Saneject.Experimental.Editor.Extensions
{
    public static class ComponentTraversalExtensions
    {
        private const BindingFlags BindingFlags =
            System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.FlattenHierarchy;

        public static IEnumerable<FieldTraversalResult> GetInjectionFieldsDeep(this Component component)
        {
            return WalkFields(component, string.Empty);
        }

        public static IEnumerable<MethodTraversalResult> GetInjectionMethodsDeep(this Component component)
        {
            return WalkMethods(component, string.Empty);
        }

        private static IEnumerable<FieldTraversalResult> WalkFields(
            object owner,
            string path)
        {
            if(owner == null)
                yield break;
            
            Type type = owner.GetType();

            foreach (FieldInfo field in type.GetFields(BindingFlags))
            {
                string fieldPath =
                    string.IsNullOrEmpty(path)
                        ? field.Name
                        : $"{path}.{field.Name}";

                if (field.TryGetAttribute(out InjectAttribute injectAttribute))
                    yield return new FieldTraversalResult(owner, field, fieldPath, injectAttribute);

                if (!field.FieldType.IsNestedSerializable())
                    continue;

                object nested = field.GetValue(owner);

                if (nested == null)
                    continue;

                foreach (FieldTraversalResult child in WalkFields(nested, fieldPath))
                    yield return child;
            }
        }

        private static IEnumerable<MethodTraversalResult> WalkMethods(
            object owner,
            string path)
        {
            if(owner == null)
                yield break;
            
            Type type = owner.GetType();

            foreach (MethodInfo method in type.GetMethods(BindingFlags))
            {
                string methodPath =
                    string.IsNullOrEmpty(path)
                        ? method.Name
                        : $"{path}.{method.Name}";

                if (method.TryGetAttribute(out InjectAttribute injectAttribute))
                    yield return new MethodTraversalResult(owner, method, methodPath, injectAttribute);
            }

            foreach (FieldInfo field in type.GetFields(BindingFlags))
            {
                if (!field.FieldType.IsNestedSerializable())
                    continue;

                object nested = field.GetValue(owner);

                if (nested == null)
                    continue;

                string nestedPath =
                    string.IsNullOrEmpty(path)
                        ? field.Name
                        : $"{path}.{field.Name}";

                foreach (MethodTraversalResult child in WalkMethods(nested, nestedPath))
                    yield return child;
            }
        }
    }
}