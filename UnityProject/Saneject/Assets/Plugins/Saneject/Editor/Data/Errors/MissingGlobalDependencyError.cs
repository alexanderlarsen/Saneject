using System;
using System.Collections.Generic;
using System.ComponentModel;
using Plugins.Saneject.Editor.Data.Graph.Nodes;
using Plugins.Saneject.Editor.Utilities;

namespace Plugins.Saneject.Editor.Data.Errors
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class MissingGlobalDependencyError : InjectionError
    {
        public MissingGlobalDependencyError(
            BindingNode bindingNode,
            HashSet<Type> rejectedTypes) : base
        (
            bindingNode.ScopeNode.TransformNode.Transform,
            false,
            null
        )
        {
            RejectedTypes = rejectedTypes;
            BindingSignature = SignatureUtility.GetBindingSignature(bindingNode);
        }

        public string BindingSignature { get; }
        public HashSet<Type> RejectedTypes { get; }
    }
}