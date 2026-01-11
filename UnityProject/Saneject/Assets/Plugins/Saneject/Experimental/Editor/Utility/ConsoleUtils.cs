using System;
using System.Reflection;

namespace Plugins.Saneject.Experimental.Editor.Utility
{
    public static class ConsoleUtils
    {
        public static void ClearLog()
        {
            Type logEntries = Type.GetType("UnityEditor.LogEntries, UnityEditor.dll");
            MethodInfo clearMethod = logEntries?.GetMethod("Clear", BindingFlags.Static | BindingFlags.Public);
            clearMethod?.Invoke(null, null);
        }

        public static void SetSearch(string query)
        {
            // Update backend filter
            Type logEntriesType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.LogEntries");
            MethodInfo setFilteringText = logEntriesType.GetMethod("SetFilteringText", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            setFilteringText?.Invoke(null, new object[] { query });

            // Update visible search box text
            Type consoleWindowType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.ConsoleWindow");
            FieldInfo field = consoleWindowType.GetField("ms_ConsoleWindow", BindingFlags.Static | BindingFlags.NonPublic);
            object consoleInstance = field?.GetValue(null);

            if (consoleInstance != null)
            {
                FieldInfo searchField = consoleWindowType.GetField("m_SearchText", BindingFlags.Instance | BindingFlags.NonPublic);
                searchField?.SetValue(consoleInstance, query);

                MethodInfo repaint = consoleWindowType.GetMethod("Repaint", BindingFlags.Instance | BindingFlags.Public);
                repaint?.Invoke(consoleInstance, null);
            }
        }
    }
}