using System.ComponentModel;
using Plugins.Saneject.Editor.Data.Graph.Nodes;
using Plugins.Saneject.Editor.Utilities;

namespace Plugins.Saneject.Editor.Data.Errors
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class MissingBindingError : InjectionError
    {
        public MissingBindingError(FieldNode fieldNode)
            : base
            (
                logContext: fieldNode.ComponentNode.TransformNode.Transform,
                suppressError: fieldNode.SuppressMissingErrors,
                exception: null
            )
        {
            ExpectedBindingSignature = SignatureUtility.GetHypotheticalBindingSignature
            (
                fieldNode.RequestedType,
                fieldNode.IsCollection,
                fieldNode.ComponentNode.TransformNode.NearestScopeNode
            );

            SiteSignature = SignatureUtility.GetFieldSignature(fieldNode);
        }

        public MissingBindingError(MethodParameterNode parameterNode)
            : base
            (
                logContext: parameterNode.MethodNode.ComponentNode.TransformNode.Transform,
                suppressError: parameterNode.MethodNode.SuppressMissingErrors,
                exception: null
            )
        {
            ExpectedBindingSignature = SignatureUtility.GetHypotheticalBindingSignature
            (
                parameterNode.RequestedType,
                parameterNode.IsCollection,
                parameterNode.MethodNode.ComponentNode.TransformNode.NearestScopeNode
            );

            SiteSignature = SignatureUtility.GetMethodParameterSignature(parameterNode);
        }

        public string ExpectedBindingSignature { get; }
        public string SiteSignature { get; }
    }
}