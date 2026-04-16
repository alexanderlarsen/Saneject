using System.ComponentModel;
using Plugins.Saneject.Editor.Data.Context;

namespace Plugins.Saneject.Editor.Data.BatchInjection
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class BatchItem
    {
        protected BatchItem(
            string path,
            ContextWalkFilter contextWalkFilter)
        {
            Path = path;
            ContextWalkFilter = contextWalkFilter;
        }

        public string Path { get; }
        public ContextWalkFilter ContextWalkFilter { get; }
        public InjectionStatus Status { get; set; }
    }
}