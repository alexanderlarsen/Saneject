using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Development.Editor
{
    public static class MonoBehaviourInterfaceLogger
    {
        [MenuItem(itemName: "Assets/Log All Interfaces", priority = 0)]
        private static void LogInterfaces()
        {
            MonoScript monoScript = Selection.activeObject as MonoScript;

            if (monoScript == null)
            {
                Debug.LogWarning("Selected object is not a MonoBehaviour script asset.");
                return;
            }

            Type scriptClass = monoScript.GetClass();

            if (scriptClass == null)
            {
                Debug.LogWarning("Could not get class from script. (Maybe not compiled, or not a MonoBehaviour?)");
                return;
            }

            Type[] interfaces = scriptClass.GetInterfaces();

            Debug.Log($"[{scriptClass.FullName}] implements:\n" +
                      string.Join("\n", interfaces.Select(i => i.FullName)));
        }
    }
}