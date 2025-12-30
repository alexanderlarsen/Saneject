using System.Reflection;

namespace Plugins.Saneject.Experimental.Editor.Graph
{
    public class MethodNode : InjectionSiteNode
    {
        public MethodNode(
            MethodInfo methodInfo,
            ComponentNode componentNode) : base(methodInfo, componentNode)
        {
            MethodInfo = methodInfo;
        }

        public MethodInfo MethodInfo { get; }
    }
}