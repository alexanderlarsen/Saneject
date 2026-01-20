namespace Plugins.Saneject.Experimental.Editor.Data.BatchInjection
{
    public abstract class BatchItem
    {
        protected BatchItem(string path)
        {
            Path = path;
        }

        public string Path { get; }
    }
}