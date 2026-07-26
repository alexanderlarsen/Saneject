using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Plugins.Saneject.Editor.Data.Graph;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Editor.Extensions
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class TypeExtensions
    {
        /// <summary>
        /// Returns the element type of a single or collection type (array/list).
        /// </summary>
        public static Type ResolveElementType(this Type type)
        {
            if (type.IsArray)
                return type.GetElementType();

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
                return type.GetGenericArguments()[0];

            return type;
        }

        public static TypeShape GetTypeShape(this Type type)
        {
            if (type.IsArray)
                return TypeShape.Array;

            return typeof(IList).IsAssignableFrom(type)
                ? TypeShape.List
                : TypeShape.Single;
        }

        public static bool IsNestedSerializable(this Type type)
        {
            if (type == null)
                return false;

            if (!type.IsClass)
                return false;

            if (type == typeof(string))
                return false;

            // Unity does not serialize generic types (e.g. Dictionary<,>, List<T>) as nested custom
            // classes, and their internal fields can form reference cycles (Dictionary <-> ValueCollection),
            // so never recurse into them during traversal.
            if (type.IsGenericType)
                return false;

            return !typeof(Object).IsAssignableFrom(type) &&
                   type.IsDefined(typeof(SerializableAttribute), inherit: false);
        }
    }
}