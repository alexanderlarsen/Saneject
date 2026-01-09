using System.Reflection;

namespace Plugins.Saneject.Experimental.Editor.Data
{
    public readonly struct FieldTraversalResult
    {
        public FieldTraversalResult(
            object owner,
            FieldInfo fieldInfo,
            string path)
        {
            Owner = owner;
            FieldInfo = fieldInfo;
            Path = path;
        }

        public object Owner { get; }
        public FieldInfo FieldInfo { get; }
        public string Path { get; }
    }
}