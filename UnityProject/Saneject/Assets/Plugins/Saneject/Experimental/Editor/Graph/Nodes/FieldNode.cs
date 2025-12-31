using System;
using System.Reflection;
using Plugins.Saneject.Experimental.Editor.Extensions;
using Plugins.Saneject.Experimental.Editor.Utils.Plugins.Saneject.Experimental.Editor.Utils;

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
            DisplayPath = FieldNodeUtils.GetPath(this);
            IsPropertyBackingField = fieldInfo.Name.Contains(">k__BackingField");
        }

        public FieldInfo FieldInfo { get; }

        public Type InterfaceType { get; }
        public Type ConcreteType { get; }
        public bool IsCollection { get; }
        public string DisplayPath { get; }
        public bool IsPropertyBackingField { get; }
    }
}