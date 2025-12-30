using System.Reflection;
using Plugins.Saneject.Runtime.Attributes;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Editor.Graph.Nodes
{
    public abstract class InjectionSiteNode
    {
        protected InjectionSiteNode(
            MemberInfo memberInfo,
            ComponentNode componentNode)
        {
            ComponentNode = componentNode;
            InjectAttribute injectAttribute = memberInfo.GetCustomAttribute<InjectAttribute>();
            InjectId = injectAttribute.ID;
            SuppressMissingErrors = injectAttribute.SuppressMissingErrors;
        }
        
        public ComponentNode ComponentNode { get; }
        public Component Component => ComponentNode.Component;
        
        public string InjectId { get; }
        public bool SuppressMissingErrors { get; }
    }
}