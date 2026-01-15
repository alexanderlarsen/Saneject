using System;
using System.Reflection;
using Plugins.Saneject.Experimental.Editor.Extensions;
using Plugins.Saneject.Experimental.Editor.Utilities;
using Plugins.Saneject.Experimental.Runtime.Attributes;

namespace Plugins.Saneject.Experimental.Editor.Graph.Nodes
{
    public abstract class MemberNode
    {
        protected MemberNode(
            object owner,
            MemberInfo memberInfo,
            ComponentNode componentNode,
            string pathFromComponent,
            InjectAttribute injectAttribute)
        {
            Owner = owner;
            ComponentNode = componentNode;
            DeclaringType = memberInfo.DeclaringType;
            QualifyingName = BackingFieldNameUtility.GetLogicalName(memberInfo.Name);
            InjectId = injectAttribute.ID;
            SuppressMissingErrors = injectAttribute.SuppressMissingErrors;
            DisplayPath = this.GetDisplayPath(pathFromComponent);
        }

        public object Owner { get; }
        public ComponentNode ComponentNode { get; }
        public Type DeclaringType { get; }
        public string QualifyingName { get; }
        public string InjectId { get; }
        public bool SuppressMissingErrors { get; }
        public string DisplayPath { get; }
    }
}