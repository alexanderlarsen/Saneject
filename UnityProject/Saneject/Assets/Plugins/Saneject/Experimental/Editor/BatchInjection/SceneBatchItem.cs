namespace Plugins.Saneject.Experimental.Editor.Data
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