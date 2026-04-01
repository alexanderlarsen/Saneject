using System;
using System.Collections.Generic;
using Plugins.Saneject.Editor.Data.Graph.Nodes;
using Plugins.Saneject.Editor.Utilities;

namespace Plugins.Saneject.Editor.Data.Errors
{
    public sealed class MissingDependencyError : InjectionError
    {
        public MissingDependencyError(
            BindingNode bindingNode,
            FieldNode fieldNode,
            HashSet<Type> rejectedTypes) : base
        (
            logContext: bindingNode.ScopeNode.TransformNode.Transform,
            suppressError: fieldNode.SuppressMissingErrors,
            exception: null
        )
        {
            BindingSignature = SignatureUtility.GetBindingSignature(bindingNode);
            SiteSignature = SignatureUtility.GetFieldSignature(fieldNode);
            IsCollection = fieldNode.IsCollection;
            RejectedTypes = rejectedTypes;
        }

        public MissingDependencyError(
            BindingNode bindingNode,
            MethodParameterNode parameterNode,
            HashSet<Type> rejectedTypes) : base
        (
            logContext: bindingNode.ScopeNode.TransformNode.Transform,
            suppressError: parameterNode.MethodNode.SuppressMissingErrors,
            exception: null
        )
        {
            BindingSignature = SignatureUtility.GetBindingSignature(bindingNode);
            SiteSignature = SignatureUtility.GetMethodParameterSignature(parameterNode);
            IsCollection = parameterNode.IsCollection;
            RejectedTypes = rejectedTypes;
        }

        public string BindingSignature { get; }
        public string SiteSignature { get; }
        public bool IsCollection { get; }
        public HashSet<Type> RejectedTypes { get; }
    }
}