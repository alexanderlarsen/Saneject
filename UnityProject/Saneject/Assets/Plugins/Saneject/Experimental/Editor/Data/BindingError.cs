using Plugins.Saneject.Experimental.Editor.Utils;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Editor.Data
{
    public class BindingError
    {
        public BindingError(
            BindingContext context,
            string errorMessage)
        {
            Transform = context.Transform;
            ErrorMessage = $"Invalid binding {BindingSignatureBuilder.GetBindingSignature(context)}: {errorMessage}";
        }

        public Transform Transform { get; }
        public string ErrorMessage { get; }
    }
}