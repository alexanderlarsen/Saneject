namespace Plugins.Saneject.Experimental.Runtime.Bindings.Component
{
    public class GlobalComponentBindingBuilder<TComponent> : BaseComponentBindingBuilder<TComponent> where TComponent : class
    {
        public GlobalComponentBindingBuilder(ComponentBinding binding) : base(binding)
        {
        }
    }
}