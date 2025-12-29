using System.Reflection;
using Plugins.Saneject.Runtime.Attributes;

namespace Plugins.Saneject.Experimental.Editor.Graph
{
    public class MethodNode
    {
        public MethodNode(
            object owner,
            MethodInfo methodInfo)
        {
            Owner = owner;
            MethodInfo = methodInfo;
            InjectAttribute injectAttribute = methodInfo.GetCustomAttribute<InjectAttribute>();
            InjectId = injectAttribute.ID;
            SuppressMissingErrors = injectAttribute.SuppressMissingErrors;
        }

        public object Owner { get; }
        public MethodInfo MethodInfo { get; }
        public string InjectId { get; }
        public bool SuppressMissingErrors { get; }
    }
}