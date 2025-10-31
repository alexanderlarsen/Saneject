using System;

namespace Plugins.Saneject.Editor.EditorWindows.BatchInjector
{
    [Serializable]
    public class AssetEntry
    {
        public AssetEntry(
            string path,
            bool enabled)
        {
            Path = path;
            Enabled = enabled;
            Name = System.IO.Path.GetFileNameWithoutExtension(path);
        }

        public string Path;
        public string Name;
        public bool Enabled;
    }
}