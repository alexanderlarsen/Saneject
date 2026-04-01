using System;
using Plugins.Saneject.Editor.Data.Graph.Nodes;
using Plugins.Saneject.Editor.Utilities;

namespace Plugins.Saneject.Editor.Data.Errors
{
    public sealed  class InvalidBindingError : InjectionError
    {
        public InvalidBindingError(
            BindingNode bindingNode,
            string reason,
            Exception exception = null)
            : base
            (
                logContext: bindingNode.ScopeNode.TransformNode.Transform,
                suppressError: false,
                exception: exception
            )
        {
            Reason = reason;
            BindingSignature = SignatureUtility.GetBindingSignature(bindingNode);
        }

        public string Reason { get; }
        public string BindingSignature { get; }
    }
}