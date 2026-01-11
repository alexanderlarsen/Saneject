using System;
using System.Reflection;

namespace Plugins.Saneject.Legacy.Editor.Extensions
{
    public static class FieldInfoExtensions
    {
        public static bool TryGetAttribute<T>(
            this MemberInfo memberInfo,
            out T attribute,
            bool inherit = false) where T : Attribute
        {
            attribute = memberInfo.GetCustomAttribute<T>(inherit);
            return attribute != null;
        }

        public static bool HasAttribute<T>(this MemberInfo memberInfo) where T : Attribute
        {
            return memberInfo.TryGetAttribute<T>(out _);
        }
    }
}