using Plugins.Saneject.Experimental.Editor.Graph.Nodes;
using Plugins.Saneject.Experimental.Editor.Utils;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Editor.Data
{
    public class BindingError
    {
        private BindingError(
            string errorMessage,
            Transform transform)
        {
            Transform = transform;
            ErrorMessage = errorMessage;
        }

        public Transform Transform { get; }
        public string ErrorMessage { get; }

        public static BindingError CreateInvalidConfigError(
            string errorMessage,
            BindingNode bindingNode)
        {
            return new BindingError
            (
                $"Invalid binding {SignatureBuilder.GetBindingSignature(bindingNode)} {errorMessage}",
                bindingNode.ScopeNode.TransformNode.Transform
            );
        }

        public static BindingError CreateMissingBindingError(FieldNode fieldNode)
        {
            string expectedBindingSignature = SignatureBuilder.GetPartialBindingSignature(fieldNode.IsCollection, fieldNode.InterfaceType, fieldNode.ConcreteType, fieldNode.ComponentNode.TransformNode.NearestScopeNode);

            return new BindingError
            (
                $"Missing binding {expectedBindingSignature} {SignatureBuilder.GetFieldSignature(fieldNode)}",
                fieldNode.ComponentNode.TransformNode.Transform
            );
        }
    }
}