using Plugins.Saneject.Experimental.Editor.Data.Context;

namespace Plugins.Saneject.Experimental.Editor.Data.BatchInjection
{
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