using Plugins.Saneject.Experimental.Editor.Graph.Nodes;
using Plugins.Saneject.Experimental.Editor.Utils;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Editor.Data
{
    public class BindingError
    {
        public BindingError(
            BindingNode bindingNode,
            string errorMessage)
        {
            Transform = bindingNode.ScopeNode.TransformNode.Transform;
            ErrorMessage = $"Invalid binding {BindingSignatureBuilder.GetBindingSignature(bindingNode)}: {errorMessage}";
        }

        public Transform Transform { get; }
        public string ErrorMessage { get; }
    }
}