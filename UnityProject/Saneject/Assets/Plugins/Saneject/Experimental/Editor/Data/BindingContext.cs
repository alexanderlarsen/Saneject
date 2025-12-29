using Plugins.Saneject.Experimental.Runtime.Bindings;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Editor.Data
{
    public class BindingContext
    {
        public BindingContext(
            BaseBinding binding,
            Transform declaringTransform)
        {
            Binding = binding;
            DeclaringTransform = declaringTransform;
        }

        public BaseBinding Binding { get; }
        public Transform DeclaringTransform { get; }
    }
}