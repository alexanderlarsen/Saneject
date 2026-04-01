using System;
using Plugins.Saneject.Editor.Data.Graph.Nodes;
using Plugins.Saneject.Editor.Utilities;

namespace Plugins.Saneject.Editor.Data.Errors
{
    public sealed class InjectMethodInvocationError : InjectionError
    {
        public InjectMethodInvocationError(
            MethodNode methodNode,
            Exception exception)
            : base
            (
                logContext: methodNode.ComponentNode.TransformNode.Transform,
                suppressError: false,
                exception: exception
            )
        {
            SiteSignature = SignatureUtility.GetMethodSignature(methodNode);
        }

        public string SiteSignature { get; }
    }
}