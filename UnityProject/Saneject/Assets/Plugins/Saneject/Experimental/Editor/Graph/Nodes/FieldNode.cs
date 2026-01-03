using System;
using System.Collections;
using System.Reflection;
using Plugins.Saneject.Experimental.Editor.Data;
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
            RequestedType = fieldInfo.ResolveType();
            IsInterface = fieldInfo.FieldType.IsInterface;
            DisplayPath = FieldNodeUtility.GetPath(this);
            IsPropertyBackingField = fieldInfo.Name.Contains(">k__BackingField");
            FieldShape = GetFieldShape(fieldInfo);
        }

        public FieldInfo FieldInfo { get; }

        /// <summary>
        /// This is the type that the field is requesting for injection, which can either be a concrete type or an interface as the direct field type or element/generic type of an array or list.
        /// </summary>
        public Type RequestedType { get; }

        public FieldShape FieldShape { get; }
        public bool IsCollection => FieldShape is FieldShape.Array or FieldShape.List;
        public bool IsInterface { get; }
        public string DisplayPath { get; }
        public bool IsPropertyBackingField { get; }

        private static FieldShape GetFieldShape(FieldInfo fieldInfo)
        {
            if (fieldInfo.FieldType.IsArray)
                return FieldShape.Array;

            return typeof(IList).IsAssignableFrom(fieldInfo.FieldType)
                ? FieldShape.List
                : FieldShape.Single;
        }
    }
}