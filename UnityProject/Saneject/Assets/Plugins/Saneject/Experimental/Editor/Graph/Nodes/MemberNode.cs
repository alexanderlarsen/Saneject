using System;
using System.Reflection;
using Plugins.Saneject.Experimental.Editor.Utils;
using Plugins.Saneject.Runtime.Attributes;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Editor.Graph.Nodes
{
    public abstract class MemberNode
    {
        protected MemberNode(
            MemberInfo memberInfo,
            ComponentNode componentNode)
        {
            ComponentNode = componentNode;
            Component = componentNode.Component;
            DeclaringType = memberInfo.DeclaringType;

            MemberName = memberInfo.Name;
            InjectAttribute injectAttribute = memberInfo.GetCustomAttribute<InjectAttribute>();
            InjectId = injectAttribute.ID;
            SuppressMissingErrors = injectAttribute.SuppressMissingErrors;
            DisplayPath = MemberNodeUtility.GetInjectionSitePath(this);
        }

        public ComponentNode ComponentNode { get; }
        public Component Component { get; }
        public Type DeclaringType { get; }
        public string MemberName { get; }
        public string InjectId { get; }
        public bool SuppressMissingErrors { get; }
        public string DisplayPath { get; }
    }
}