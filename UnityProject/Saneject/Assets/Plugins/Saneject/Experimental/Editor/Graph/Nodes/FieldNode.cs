using System;
using System.Reflection;
using Plugins.Saneject.Experimental.Editor.Data;
using Plugins.Saneject.Experimental.Editor.Extensions;

namespace Plugins.Saneject.Experimental.Editor.Graph.Nodes
{
    public class FieldNode : MemberNode
    {
        public FieldNode(
            object owner,
            FieldInfo fieldInfo,
            ComponentNode componentNode,
            string pathFromComponent)
            : base(owner, fieldInfo, componentNode, pathFromComponent)
        {
            FieldInfo = fieldInfo;
            FieldType = fieldInfo.FieldType;
            RequestedType = fieldInfo.FieldType.ResolveElementType();
            IsInterface = RequestedType.IsInterface;
            IsPropertyBackingField = fieldInfo.Name.Contains(">k__BackingField");
            TypeShape = fieldInfo.FieldType.GetTypeShape();
            IsCollection = TypeShape is TypeShape.Array or TypeShape.List;
        }

        public FieldInfo FieldInfo { get; }
        public Type FieldType { get; }
        public Type RequestedType { get; }
        public TypeShape TypeShape { get; }
        public bool IsCollection { get; }
        public bool IsInterface { get; }
        public bool IsPropertyBackingField { get; }
    }
}