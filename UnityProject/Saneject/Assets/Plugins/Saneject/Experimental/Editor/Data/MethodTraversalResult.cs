using System.Reflection;

namespace Plugins.Saneject.Experimental.Editor.Data
{
    public readonly struct MethodTraversalResult
    {
        public MethodTraversalResult(
            object owner,
            MethodInfo methodInfo,
            string path)
        {
            Owner = owner;
            MethodInfo = methodInfo;
            Path = path;
        }

        public object Owner { get; }
        public MethodInfo MethodInfo { get; }
        public string Path { get; }
    }

}