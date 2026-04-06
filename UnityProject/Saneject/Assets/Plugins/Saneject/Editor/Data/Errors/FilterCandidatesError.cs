using System;
using System.ComponentModel;
using Plugins.Saneject.Editor.Data.Graph.Nodes;
using Plugins.Saneject.Editor.Utilities;

namespace Plugins.Saneject.Editor.Data.Errors
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class FilterCandidatesError : InjectionError
    {
        public FilterCandidatesError(
            BindingNode bindingNode,
            Exception exception)
            : base
            (
                logContext: bindingNode.ScopeNode.TransformNode.Transform,
                suppressError: false,
                exception: exception
            )
        {
            BindingSignature = SignatureUtility.GetBindingSignature(bindingNode);
        }

        public string BindingSignature { get; }
    }
}