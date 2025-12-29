using Plugins.Saneject.Experimental.GraphSystem.Bindings;
using Plugins.Saneject.Experimental.GraphSystem.Data.Nodes;
using UnityEngine;

namespace Plugins.Saneject.Experimental.GraphSystem.Data
{
    public class BindingContext
    {
        public BindingContext(
            NewBinding binding,
            Transform declaringTransform)
        {
            Binding = binding;
            DeclaringTransform = declaringTransform;
        }

        public NewBinding Binding { get; }
        public Transform DeclaringTransform { get; }
    }
}