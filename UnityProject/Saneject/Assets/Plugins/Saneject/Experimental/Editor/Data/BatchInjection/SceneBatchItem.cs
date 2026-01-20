using Plugins.Saneject.Experimental.Editor.Data.Context;

namespace Plugins.Saneject.Experimental.Editor.Data.BatchInjection
{
    public class SceneBatchItem : BatchItem
    {
        public SceneBatchItem(
            string path,
            ContextWalkFilter contextWalkFilter)
            : base(path)
        {
            ContextWalkFilter = contextWalkFilter;
        }

        public ContextWalkFilter ContextWalkFilter { get; }
    }
}