using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Plugins.Saneject.Experimental.Editor.Graph.Nodes
{
    public class MethodNode : MemberNode
    {
        public MethodNode(
            MethodInfo methodInfo,
            ComponentNode componentNode) : base(methodInfo, componentNode)
        {
            MethodInfo = methodInfo;

            ParameterNodes = methodInfo
                .GetParameters()
                .Select(parameter => new MethodParameterNode(parameter, this))
                .ToList();
        }

        public MethodInfo MethodInfo { get; }
        public IReadOnlyList<MethodParameterNode> ParameterNodes { get; }
    }
}