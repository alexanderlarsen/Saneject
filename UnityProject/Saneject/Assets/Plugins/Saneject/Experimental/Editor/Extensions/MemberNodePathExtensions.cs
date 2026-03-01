using System.ComponentModel;
using Plugins.Saneject.Experimental.Editor.Data.Graph.Nodes;
using Plugins.Saneject.Experimental.Editor.Utilities;

namespace Plugins.Saneject.Experimental.Editor.Extensions
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class MemberNodePathExtensions
    {
        public static string GetDisplayPath(
            this MemberNode node,
            string pathFromComponent)
        {
            pathFromComponent = pathFromComponent.StripPropertyBackingFieldSyntax();
            ComponentNode componentNode = node.ComponentNode;
            string goPath = GetHierarchyPath(componentNode.TransformNode);
            string componentName = componentNode.Component.GetType().Name;
            return $"{goPath}/{componentName}/{pathFromComponent}";
        } 

        public static string StripPropertyBackingFieldSyntax(this string pathFromComponent)
        {
            string[] pathParts = pathFromComponent.Split('.');

            for (int i = 0; i < pathParts.Length; i++)
                pathParts[i] = NameUtility.GetLogicalName(pathParts[i]);

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