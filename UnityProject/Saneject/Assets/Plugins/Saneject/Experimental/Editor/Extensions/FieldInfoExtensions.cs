using System;
using System.Collections.Generic;
using System.Reflection;

namespace Plugins.Saneject.Experimental.Editor.Extensions
{
    public static class FieldInfoExtensions
    {
        public static bool IsCollection(this FieldInfo info)
        {
            Type type = info.FieldType;

            bool isArray = type.IsArray;
            bool isList = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);

            return isArray || isList;
        }
        
        /// <summary>
        /// Returns the element type of a single or collection type (array/list).
        /// </summary>
        public static Type ResolveType(this FieldInfo fieldInfo)
        {
            Type fieldType = fieldInfo.FieldType;

            if (fieldType.IsArray)
                return fieldType.GetElementType();

            if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(List<>))
                return fieldType.GetGenericArguments()[0];

            return fieldType;
        }
    }
}