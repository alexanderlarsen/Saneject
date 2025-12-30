using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Experimental.Runtime.Bindings.Asset;
using Plugins.Saneject.Experimental.Runtime.Bindings.Component;

namespace Plugins.Saneject.Experimental.Runtime.Bindings
{
    public class BindingCollection
    {
        public BindingCollection(
            IReadOnlyCollection<ComponentBinding> componentBindings,
            IReadOnlyCollection<AssetBinding> assetBindings,
            IReadOnlyCollection<GlobalComponentBinding> globalBindings)
        {
            ComponentBindings = componentBindings.ToList();
            AssetBindings = assetBindings.ToList();
            GlobalBindings = globalBindings.ToList();
        }

        public IReadOnlyCollection<ComponentBinding> ComponentBindings { get; }
        public IReadOnlyCollection<AssetBinding> AssetBindings { get; }
        public IReadOnlyCollection<GlobalComponentBinding> GlobalBindings { get; }
    }
}