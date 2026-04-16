using System;
using System.ComponentModel;
using System.Reflection;

namespace Plugins.Saneject.Editor.Extensions
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class MemberInfoExtensions
    {
        public static bool TryGetAttribute<T>(
            this MemberInfo memberInfo,
            out T attribute,
            bool inherit = false) where T : Attribute
        {
            attribute = memberInfo.GetCustomAttribute<T>(inherit);
            return attribute != null;
        }
    }
}