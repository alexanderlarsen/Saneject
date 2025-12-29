using Plugins.Saneject.Experimental.Editor.Utils;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Editor.Data
{
    public class BindingError
    {
        public BindingError(
            string errorMessage,
            BindingContext context)
        {
            PingObject = context.DeclaringTransform;
            ErrorMessage = $"Invalid binding {BindingSignatureBuilder.GetBindingSignature(context.Binding)}: {errorMessage}";
        }

        public string ErrorMessage { get; }
        public Object PingObject { get; }
    }
}