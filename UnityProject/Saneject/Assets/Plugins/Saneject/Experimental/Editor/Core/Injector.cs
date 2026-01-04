using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Experimental.Editor.Data;
using Plugins.Saneject.Experimental.Editor.Graph.Nodes;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Experimental.Editor.Core
{
    public static class Injector
    {
        public static void InjectDependencies(InjectionSession session)
        {
            InjectGlobals(session);
            InjectFields(session);
        }

        private static void InjectGlobals(InjectionSession session)
        {
            IEnumerable<ScopeNode> allScopes = session.Graph
                .EnumerateAllTransformNodes()
                .Select(scopeNode => scopeNode.DeclaredScopeNode)
                .Where(scopeNode => scopeNode != null);

            foreach (ScopeNode scopeNode in allScopes)
            {
                IEnumerable<Component> globalObjects = session.GlobalResolutionMap.TryGetValue(scopeNode, out IReadOnlyList<Object> objects)
                    ? objects.Cast<Component>()
                    : Enumerable.Empty<Component>();

                scopeNode.Scope.UpdateGlobalComponents(globalObjects);
                EditorUtility.SetDirty(scopeNode.Scope);
            }
        }

        private static void InjectFields(InjectionSession session)
        {
            IEnumerable<ComponentNode> allComponents = session.Graph
                .EnumerateAllTransformNodes()
                .SelectMany(node => node.ComponentNodes);

            foreach (ComponentNode componentNode in allComponents)
            {
                foreach (FieldNode fieldNode in componentNode.FieldNodes)
                {
                    IEnumerable<Object> dependencies = session.FieldResolutionMap.TryGetValue(fieldNode, out IReadOnlyList<Object> objects)
                        ? objects
                        : Enumerable.Empty<Object>();

                    fieldNode.FieldInfo.SetValue
                    (
                        componentNode.Component,
                        ConvertDependencies(fieldNode, dependencies)
                    );
                }

                EditorUtility.SetDirty(componentNode.Component);
            }
        }

        private static object ConvertDependencies(
            FieldNode fieldNode,
            IEnumerable<Object> dependencies)
        {
            Object[] depsArray = dependencies.ToArray();

            if (depsArray.Length == 0)
                return null;

            switch (fieldNode.FieldShape)
            {
                case FieldShape.Single:
                {
                    return depsArray.First();
                }

                case FieldShape.Array:
                {
                    Array array = Array.CreateInstance(fieldNode.RequestedType, depsArray.Length);

                    for (int i = 0; i < depsArray.Length; i++)
                        array.SetValue(depsArray[i], i);

                    return array;
                }

                case FieldShape.List:
                {
                    IList list = (IList)Activator.CreateInstance(fieldNode.FieldInfo.FieldType);

                    foreach (Object d in depsArray)
                        list.Add(d);

                    return list;
                }

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}