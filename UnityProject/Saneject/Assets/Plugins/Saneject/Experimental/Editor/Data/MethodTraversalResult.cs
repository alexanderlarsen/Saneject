using System.Reflection;
using Plugins.Saneject.Experimental.Runtime.Attributes;

namespace Plugins.Saneject.Experimental.Editor.Data
{
    public readonly struct MethodTraversalResult
    {
        public MethodTraversalResult(
            object owner,
            MethodInfo methodInfo,
            string path,
            InjectAttribute injectAttribute)
        {
            Owner = owner;
            MethodInfo = methodInfo;
            Path = path;
            InjectAttribute = injectAttribute;
        }

        public object Owner { get; }
        public MethodInfo MethodInfo { get; }
        public string Path { get; }
        public InjectAttribute InjectAttribute { get; }
    }

}