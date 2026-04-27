using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Plugins.Saneject.Editor.Data.TraversalResults;
using Plugins.Saneject.Runtime.Attributes;
using Component = UnityEngine.Component;

// ReSharper disable LoopCanBePartlyConvertedToQuery

namespace Plugins.Saneject.Editor.Extensions
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ComponentTraversalExtensions
    {
        private const BindingFlags Flags =
            BindingFlags.Public |
            BindingFlags.NonPublic |
            BindingFlags.Instance |
            BindingFlags.DeclaredOnly;

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
            if (owner == null)
                yield break;

            foreach (Type type in GetTraversalTypes(owner.GetType()))
            {
                foreach (FieldInfo field in type.GetFields(Flags))
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
        }

        private static IEnumerable<MethodTraversalResult> WalkMethods(
            object owner,
            string path)
        {
            if (owner == null)
                yield break;

            foreach (Type type in GetTraversalTypes(owner.GetType()))
            {
                foreach (MethodInfo method in type.GetMethods(Flags))
                {
                    string methodPath =
                        string.IsNullOrEmpty(path)
                            ? method.Name
                            : $"{path}.{method.Name}";

                    if (method.TryGetAttribute(out InjectAttribute injectAttribute))
                        yield return new MethodTraversalResult(owner, method, methodPath, injectAttribute);
                }
            }

            foreach (Type type in GetTraversalTypes(owner.GetType()))
            {
                foreach (FieldInfo field in type.GetFields(Flags))
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

        private static IEnumerable<Type> GetTraversalTypes(Type type)
        {
            while (type != null &&
                   type != typeof(object) &&
                   type != typeof(UnityEngine.Object) &&
                   type != typeof(Component) &&
                   type != typeof(UnityEngine.MonoBehaviour))
            {
                yield return type;
                type = type.BaseType;
            }
        }
    }
}
