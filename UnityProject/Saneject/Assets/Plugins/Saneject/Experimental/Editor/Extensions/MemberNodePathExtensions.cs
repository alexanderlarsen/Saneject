using System;
using Plugins.Saneject.Experimental.Editor.Graph.Nodes;

namespace Plugins.Saneject.Experimental.Editor.Extensions
{
    public static class MemberNodePathExtensions
    {
        public static string GetDisplayPath(
            this MemberNode node,
            string pathFromComponent)
        {
            pathFromComponent = StripPropertyBackingFieldPrefix(pathFromComponent);
            ComponentNode componentNode = node.ComponentNode;
            string goPath = GetHierarchyPath(componentNode.TransformNode);
            string componentName = componentNode.Component.GetType().Name;
            return $"{goPath}/{componentName}/{pathFromComponent}";
        }

        private static string StripPropertyBackingFieldPrefix(string pathFromComponent)
        {
            string[] pathParts = pathFromComponent.Split('.');

            for (int i = 0; i < pathParts.Length; i++)
            {
                string part = pathParts[i];

                if (part.Length > 0 && part[0] == '<')
                {
                    int end = part.IndexOf(">k__BackingField", StringComparison.Ordinal);

                    if (end > 1)
                        part = part[1..end];
                }

                pathParts[i] = part;
            }

            return string.Join(".", pathParts);
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