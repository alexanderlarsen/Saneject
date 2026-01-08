using System;
using Plugins.Saneject.Experimental.Editor.Graph.Nodes;

namespace Plugins.Saneject.Experimental.Editor.Utils
{
    public static class MemberNodeUtility
    {
        public static string GetInjectionSitePath(MemberNode node)
        {
            return node switch
            {
                FieldNode fieldNode => GetFieldPath(fieldNode),
                MethodNode methodNode => GetMethodPath(methodNode),
                _ => throw new ArgumentOutOfRangeException(nameof(node), node, null)
            };
        }
        
        public static string GetFieldPath(FieldNode node)
        {
            ComponentNode componentNode = node.ComponentNode;
            string goPath = GetHierarchyPath(componentNode.TransformNode);
            string componentName = componentNode.Component.GetType().Name;
            string memberName = StripPropertyBackingFieldPrefix(node.MemberName);
            return $"{goPath}/{componentName}/{memberName}";
        }

        public static string GetMethodPath(MethodNode node)
        {
            ComponentNode componentNode = node.ComponentNode;
            string goPath = GetHierarchyPath(componentNode.TransformNode);
            string componentName = componentNode.Component.GetType().Name;
            return $"{goPath}/{componentName}/{node.MemberName}";
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