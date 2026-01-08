using System;
using Plugins.Saneject.Experimental.Editor.Graph.Nodes;

namespace Plugins.Saneject.Experimental.Editor.Utils
{
    public static class InjectionSiteUtility
    {
        public static string GetInjectionSitePath(InjectionSiteNode injectionSiteNode)
        {
            return injectionSiteNode switch
            {
                FieldNode fieldNode => GetFieldPath(fieldNode),
                MethodNode methodNode => GetMethodPath(methodNode),
                _ => throw new ArgumentOutOfRangeException(nameof(injectionSiteNode), injectionSiteNode, null)
            };
        }
        
        public static string GetFieldPath(FieldNode fieldNode)
        {
            ComponentNode componentNode = fieldNode.ComponentNode;
            string goPath = GetHierarchyPath(componentNode.TransformNode);
            string componentName = componentNode.Component.GetType().Name;
            string memberName = StripPropertyBackingFieldPrefix(fieldNode.MemberName);
            return $"{goPath}/{componentName}/{memberName}";
        }

        public static string GetMethodPath(MethodNode methodNode)
        {
            return "";
        }

        private static string StripPropertyBackingFieldPrefix(string memberName)
        {
            if (memberName.Length > 0 && memberName[0] == '<')
            {
                int end = memberName.IndexOf(">k__BackingField", StringComparison.Ordinal);

                if (end > 1)
                    return memberName[1..end];
            }

            return memberName;
        }

        private static string GetHierarchyPath(TransformNode transformNode)
        {
            TransformNode parent = transformNode.ParentTransformNode;

            return parent == null
                ? transformNode.Transform.name
                : $"{GetHierarchyPath(parent)}/{transformNode.Transform.name}";
        }
    }
}