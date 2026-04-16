using Plugins.Saneject.Runtime.Proxy;
using Plugins.Saneject.Runtime.Scopes;

namespace Plugins.Saneject.Runtime.Bindings.Component
{
    /// <summary>
    /// Builder for configuring global component bindings.
    /// Same as <see cref="ComponentBindingBuilder{TComponent}"/>, except this builder doesn't allow qualifiers and <see cref="RuntimeProxy{TComponent}"/> bindings.
    /// Components bound with this builder are added to the declaring <see cref="Scope"/> and added to the <see cref="GlobalScope"/> at runtime before any Awake methods execute.
    /// </summary>
    /// <typeparam name="TComponent">The type of the component being bound.</typeparam>
    public class GlobalComponentBindingBuilder<TComponent> : BaseComponentBindingBuilder<TComponent> where TComponent : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalComponentBindingBuilder{TComponent}"/> class.
        /// </summary>
        /// <param name="binding">The component binding to configure.</param>
        public GlobalComponentBindingBuilder(ComponentBinding binding) : base(binding)
        {
        }
    }
}