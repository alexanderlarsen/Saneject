using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Experimental.GraphSystem.Bindings;
using UnityEngine;

namespace Plugins.Saneject.Experimental.GraphSystem
{
    public abstract class NewScope : MonoBehaviour
    {
        private readonly List<NewBinding> bindings = new();

        public IReadOnlyList<NewBinding> Bindings => bindings;

        public abstract void ConfigureBindings();

        protected NewComponentBindingBuilder<T> BindComponent<T>() where T : class
        {
            NewBinding binding = new();
            binding.SetTargetType(typeof(T));
            binding.SetBindingType(BindingType.Component);
            bindings.Add(binding);
            return new NewComponentBindingBuilder<T>(binding);
        }
    }
}