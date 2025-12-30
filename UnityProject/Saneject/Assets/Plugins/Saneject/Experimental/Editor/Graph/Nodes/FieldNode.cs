using System.Reflection;
using Plugins.Saneject.Experimental.Editor.Extensions;

namespace Plugins.Saneject.Experimental.Editor.Graph.Nodes
{
    public class FieldNode : InjectionSiteNode
    {
        public FieldNode(
            FieldInfo fieldInfo,
            ComponentNode componentNode) : base(fieldInfo, componentNode)
        {
            FieldInfo = fieldInfo;
            IsCollection = fieldInfo.IsCollection();
        }

        public FieldInfo FieldInfo { get; }
        public bool IsCollection { get; }
    }
}