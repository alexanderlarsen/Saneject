using System.Reflection;
using Plugins.Saneject.Experimental.Runtime.Attributes;

namespace Plugins.Saneject.Experimental.Editor.Data
{
    public readonly struct FieldTraversalResult
    {
        public FieldTraversalResult(
            object owner,
            FieldInfo fieldInfo,
            string path,
            InjectAttribute injectAttribute)
        {
            Owner = owner;
            FieldInfo = fieldInfo;
            Path = path;
            InjectAttribute = injectAttribute;
        }

        public object Owner { get; }
        public FieldInfo FieldInfo { get; }
        public string Path { get; }
        public InjectAttribute InjectAttribute { get; }
    }
}