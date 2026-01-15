using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Plugins.Saneject.Experimental.Runtime.Attributes;

namespace Plugins.Saneject.Experimental.Editor.Graph.Nodes
{
    public class MethodNode : MemberNode
    {
        public MethodNode(
            object owner,
            MethodInfo methodInfo,
            ComponentNode componentNode,
            string pathFromComponent,
            InjectAttribute injectAttribute)
            : base(owner, methodInfo, componentNode, pathFromComponent, injectAttribute)
        {
            MethodInfo = methodInfo;
            MethodName = methodInfo.Name;

            ParameterNodes = methodInfo
                .GetParameters()
                .Select(parameter => new MethodParameterNode(parameter, this))
                .ToList();
        }

        public MethodInfo MethodInfo { get; }
        public string MethodName { get; }
        public IReadOnlyCollection<MethodParameterNode> ParameterNodes { get; }
    }
}