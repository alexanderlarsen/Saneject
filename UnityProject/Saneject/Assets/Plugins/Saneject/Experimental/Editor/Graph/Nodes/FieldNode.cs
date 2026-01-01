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
            RequestedType = fieldInfo.ResolveType();
            IsInterface = fieldInfo.FieldType.IsInterface;
            IsCollection = fieldInfo.IsCollection();
            DisplayPath = FieldNodeUtils.GetPath(this);
            IsPropertyBackingField = fieldInfo.Name.Contains(">k__BackingField");
        }

        /// <summary>
        /// This is the type that the field is requesting for injection, which can either be a concrete type or an interface as the direct field type or element/generic type of an array or list.
        /// </summary>
        public Type RequestedType { get; }

        public bool IsInterface { get; }
        public bool IsCollection { get; }
        public string DisplayPath { get; }
        public bool IsPropertyBackingField { get; }
    }
}