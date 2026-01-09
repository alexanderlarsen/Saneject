using System;
using System.Collections.Generic;
using System.Reflection;
using Plugins.Saneject.Experimental.Editor.Data;
using UnityEngine;
using Object = UnityEngine.Object;

// ReSharper disable LoopCanBePartlyConvertedToQuery

namespace Plugins.Saneject.Experimental.Editor.Extensions
{
    public static class ComponentTraversalExtensions
    {
        public static IEnumerable<FieldTraversalResult> GetFieldsDeep(
            this Component component,
            BindingFlags flags)
        {
            return WalkFields(component, flags, string.Empty);
        }

        public static IEnumerable<MethodTraversalResult> GetMethodsDeep(
            this Component component,
            BindingFlags flags)
        {
            return WalkMethods(component, flags, string.Empty);
        }

        private static IEnumerable<FieldTraversalResult> WalkFields(
            object owner,
            BindingFlags flags,
            string path)
        {
            Type type = owner.GetType();

            foreach (FieldInfo field in type.GetFields(flags))
            {
                string fieldPath =
                    string.IsNullOrEmpty(path)
                        ? field.Name
                        : $"{path}.{field.Name}";

                yield return new FieldTraversalResult(owner, field, fieldPath);

                if (!IsNestedSerializable(field.FieldType))
                    continue;

                object nested = field.GetValue(owner);

                if (nested == null)
                    continue;

                foreach (FieldTraversalResult child in WalkFields(nested, flags, fieldPath))
                    yield return child;
            }
        }

        private static IEnumerable<MethodTraversalResult> WalkMethods(
            object owner,
            BindingFlags flags,
            string path)
        {
            Type type = owner.GetType();

            foreach (MethodInfo method in type.GetMethods(flags))
            {
                string methodPath =
                    string.IsNullOrEmpty(path)
                        ? method.Name
                        : $"{path}.{method.Name}";

                yield return new MethodTraversalResult(owner, method, methodPath);
            }

            foreach (FieldInfo field in type.GetFields(flags))
            {
                if (!IsNestedSerializable(field.FieldType))
                    continue;

                object nested = field.GetValue(owner);

                if (nested == null)
                    continue;

                string nestedPath =
                    string.IsNullOrEmpty(path)
                        ? field.Name
                        : $"{path}.{field.Name}";

                foreach (MethodTraversalResult child in WalkMethods(
                             nested,
                             flags,
                             nestedPath))
                    yield return child;
            }
        }

        private static bool IsNestedSerializable(Type type)
        {
            if (type == null)
                return false;

            if (typeof(Object).IsAssignableFrom(type))
                return false;

            if (type.IsPrimitive || type == typeof(string))
                return false;

            return type.IsDefined(typeof(SerializableAttribute), inherit: false);
        }
    }
}