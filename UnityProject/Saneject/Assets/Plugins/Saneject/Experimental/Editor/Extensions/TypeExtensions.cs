using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Plugins.Saneject.Experimental.Editor.Data;

namespace Plugins.Saneject.Experimental.Editor.Extensions
{
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
    }
}