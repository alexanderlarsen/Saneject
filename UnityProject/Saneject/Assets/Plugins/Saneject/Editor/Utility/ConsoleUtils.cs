using System;
using System.Reflection;

namespace Saneject.Plugins.Saneject.Editor.Utility
{
    public static class ConsoleUtils
    {
        public static void ClearLog()
        {
            Type logEntries = Type.GetType("UnityEditor.LogEntries, UnityEditor.dll");
            MethodInfo clearMethod = logEntries?.GetMethod("Clear", BindingFlags.Static | BindingFlags.Public);
            clearMethod?.Invoke(null, null);
        }
    }
}