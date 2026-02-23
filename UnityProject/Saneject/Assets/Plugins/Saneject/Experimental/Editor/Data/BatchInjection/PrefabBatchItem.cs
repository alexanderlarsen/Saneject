using Plugins.Saneject.Experimental.Editor.Data.Context;

namespace Plugins.Saneject.Experimental.Editor.Data.BatchInjection
{
    public class PrefabBatchItem : BatchItem
    {
        public PrefabBatchItem(
            string path,
            ContextWalkFilter contextWalkFilter)
            : base(path, contextWalkFilter)
        {
        }
    }
}