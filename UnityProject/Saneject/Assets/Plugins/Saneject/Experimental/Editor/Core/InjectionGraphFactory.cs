using System.Linq;
using Plugins.Saneject.Experimental.Editor.Graph;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Editor.Core
{
    public static class InjectionGraphFactory
    {
        public static InjectionGraph CreateGraph(params GameObject[] startObjects)
        {
            return new InjectionGraph(startObjects.Select(gameObject => gameObject.transform));
        }

        public static InjectionGraph CreateGraph(params Transform[] startTransforms)
        {
            return new InjectionGraph(startTransforms);
        }

        public static InjectionGraph CreateGraph(params Component[] startComponents)
        {
            return new InjectionGraph(startComponents.Select(component => component.transform));
        }
    }
}