using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Experimental.Editor.Data;
using Plugins.Saneject.Experimental.Editor.Graph.Nodes;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Experimental.Editor.Core
{
    public static class Injector
    {
        public static void InjectDependencies(InjectionSession session)
        {
            foreach ((FieldNode fieldNode, IEnumerable<Object> dependencies) in session.FieldResolutionMap)
                fieldNode.FieldInfo.SetValue
                (
                    fieldNode.ComponentNode.Component,
                    ConvertDependencies(fieldNode, dependencies)
                );
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