using System;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Runtime.Bindings
{
    /// <summary>
    /// Fluent builder for filtering object bindings.
    /// Used to add filter conditions when resolving <c>T</c> (an <see cref="Object"/>).
    /// Returned by methods on <see cref="ObjectBindingBuilder{T}" /> to further constrain resolution results.
    /// </summary>
    /// <typeparam name="T">The <see cref="Object"/> type to filter.</typeparam>
    public class FilterableObjectBindingBuilder<T> where T : Object
    {
        private readonly Binding binding;

        public FilterableObjectBindingBuilder(Binding binding)
        {
            this.binding = binding;
        }

        /// <summary>
        /// Filter results to only <see cref="Object" />s with the specified name.
        /// </summary>
        public FilterableObjectBindingBuilder<T> WhereNameIs(string name)
        {
            binding.AddFilter(o => o.name == name);
            return this;
        }

        /// <summary>
        /// Filter results to only <see cref="Object" />s whose name contains the specified substring.
        /// </summary>
        public FilterableObjectBindingBuilder<T> WhereNameContains(string substring)
        {
            binding.AddFilter(o => o.name.Contains(substring));
            return this;
        }

        /// <summary>
        /// Filter results using a custom predicate.
        /// </summary>
        public FilterableObjectBindingBuilder<T> Where(Func<T, bool> predicate)
        {
            binding.AddFilter(o => o is T t && predicate(t));
            return this;
        }

        /// <summary>
        /// Filter results to only <see cref="Object" />s of type <typeparamref name="TO" />.
        /// </summary>
        public FilterableObjectBindingBuilder<T> WhereIsOfType<TO>() where TO : Object
        {
            binding.AddFilter(o => o is TO);
            return this;
        }
    }
}