using System;
using System.IO;

namespace Plugins.Saneject.Editor.EditorWindows.BatchInjector
{
    [Serializable]
    public class AssetEntry
    {
        public string path;
        public string name;
        public bool enabled;

        public AssetEntry(
            string path,
            bool enabled)
        {
            this.path = path;
            this.enabled = enabled;
            name = Path.GetFileNameWithoutExtension(path);
        }
    }
}