namespace Plugins.Saneject.Experimental.Editor.Data
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