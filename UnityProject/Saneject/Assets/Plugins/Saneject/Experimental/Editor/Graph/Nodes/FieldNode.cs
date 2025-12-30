using System;
using System.Reflection;
using Plugins.Saneject.Experimental.Editor.Extensions;

namespace Plugins.Saneject.Experimental.Editor.Graph.Nodes
{
    public class FieldNode : InjectionSiteNode
    {
        public FieldNode(
            FieldInfo fieldInfo,
            ComponentNode componentNode) : base(fieldInfo, componentNode)
        {
            FieldInfo = fieldInfo;
            IsCollection = fieldInfo.IsCollection();
            InterfaceType = fieldInfo.FieldType.IsInterface ? fieldInfo.FieldType : null;
            ConcreteType = fieldInfo.FieldType.IsInterface ? null : fieldInfo.FieldType;
        }

        public FieldInfo FieldInfo { get; }
        
        
        public Type InterfaceType { get; }
        public Type ConcreteType { get; }
        public bool IsCollection { get; }
    
    }
}