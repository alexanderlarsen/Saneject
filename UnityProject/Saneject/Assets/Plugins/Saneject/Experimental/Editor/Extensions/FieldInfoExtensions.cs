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
    }
}