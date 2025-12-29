using System.Reflection;
using Plugins.Saneject.Experimental.Editor.Extensions;
using Plugins.Saneject.Runtime.Attributes;

namespace Plugins.Saneject.Experimental.Editor.Graph
{
    public class FieldNode
    {
        public FieldNode(
            object owner,
            FieldInfo fieldInfo)
        {
            Owner = owner;
            FieldInfo = fieldInfo;
            InjectAttribute injectAttribute = fieldInfo.GetCustomAttribute<InjectAttribute>();
            InjectId = injectAttribute.ID;
            SuppressMissingErrors = injectAttribute.SuppressMissingErrors;
            IsCollection = fieldInfo.IsCollection();
        }

        public object Owner { get; }
        public FieldInfo FieldInfo { get; }
        public string InjectId { get; }
        public bool SuppressMissingErrors { get; }
        public bool IsCollection { get; }
    }
}