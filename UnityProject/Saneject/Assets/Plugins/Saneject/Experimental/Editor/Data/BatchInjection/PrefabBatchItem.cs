using System.ComponentModel;
using Plugins.Saneject.Experimental.Editor.Data.Context;

namespace Plugins.Saneject.Experimental.Editor.Data.BatchInjection
{
    [EditorBrowsable(EditorBrowsableState.Never)]
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