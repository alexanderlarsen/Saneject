using System;
using System.Reflection;
using Plugins.Saneject.Experimental.Editor.Extensions;
using Plugins.Saneject.Runtime.Attributes;

namespace Plugins.Saneject.Experimental.Editor.Graph.Nodes
{
    public abstract class MemberNode
    {
        protected MemberNode(
            object owner,
            MemberInfo memberInfo,
            ComponentNode componentNode,
            string pathFromComponent)
        {
            Owner = owner;
            ComponentNode = componentNode;
            DeclaringType = memberInfo.DeclaringType;
            MemberName = memberInfo.Name;
            InjectAttribute injectAttribute = memberInfo.GetCustomAttribute<InjectAttribute>();
            InjectId = injectAttribute.ID;
            SuppressMissingErrors = injectAttribute.SuppressMissingErrors;
            DisplayPath = this.GetDisplayPath(pathFromComponent);
        }

        public object Owner { get; }
        public ComponentNode ComponentNode { get; }
        public Type DeclaringType { get; }
        public string MemberName { get; } // TODO: strip of compiler backing syntax, otherwise member qualifier filters wont work
        public string InjectId { get; }
        public bool SuppressMissingErrors { get; }
        public string DisplayPath { get; }
    }
}