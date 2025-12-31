using System;
using System.Collections.Generic;
using Plugins.Saneject.Experimental.Editor.Graph.Nodes;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Experimental.Editor.Core
{
    public static class DependencyLocator
    {
        public static List<Object> LocateDependencies(
            BindingNode bindingNode,
            out HashSet<Type> rejectedTypes)
        {
            List<Object> dependencies = new();
            rejectedTypes = new HashSet<Type>();
            return dependencies;
        }
    }
}