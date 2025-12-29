using Plugins.Saneject.Experimental.GraphSystem.Utils;
using UnityEngine;

namespace Plugins.Saneject.Experimental.GraphSystem.Data
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