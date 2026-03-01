using System.ComponentModel;
using Plugins.Saneject.Experimental.Editor.Data.Context;

namespace Plugins.Saneject.Experimental.Editor.Data.BatchInjection
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class SceneBatchItem : BatchItem
    {
        public SceneBatchItem(
            string path,
            ContextWalkFilter contextWalkFilter)
            : base(path, contextWalkFilter)
        {
        }
    }
}