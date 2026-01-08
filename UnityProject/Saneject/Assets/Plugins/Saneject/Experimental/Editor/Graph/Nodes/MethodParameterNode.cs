using System;
using System.Reflection;
using Plugins.Saneject.Experimental.Editor.Data;
using Plugins.Saneject.Experimental.Editor.Extensions;

namespace Plugins.Saneject.Experimental.Editor.Graph.Nodes
{
    public class MethodParameterNode
    {
        public MethodParameterNode(
            ParameterInfo parameterInfo,
            MethodNode methodNode)
        {
            MethodNode = methodNode;
            RequestedType = parameterInfo.ParameterType.ResolveElementType();
            IsInterface = RequestedType.IsInterface;
            TypeShape = parameterInfo.ParameterType.GetTypeShape();
            IsCollection = TypeShape is TypeShape.Array or TypeShape.List;
        }

        public MethodNode MethodNode { get; }
        public Type RequestedType { get; }
        public TypeShape TypeShape { get; }
        public bool IsCollection { get; }
        public bool IsInterface { get; }
    }
}