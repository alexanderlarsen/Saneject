using System;
using System.IO;

namespace Plugins.Saneject.Editor.EditorWindows.BatchInjection
{
    [Serializable]
    public class AssetEntry
    {
        public string path;
        public string name;
        public bool enabled = true;
        
        public AssetEntry(string path)
        {
            this.path = path;
            name = Path.GetFileNameWithoutExtension(path);
        }
    }
}