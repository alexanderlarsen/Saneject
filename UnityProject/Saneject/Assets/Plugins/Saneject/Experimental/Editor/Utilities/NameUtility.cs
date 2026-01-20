using System;

namespace Plugins.Saneject.Experimental.Editor.Utilities
{
    public static class NameUtility
    {
        /// <summary>
        /// Returns the property name associated with a compiler-generated
        /// auto-property backing field (e.g. "&lt;Name&gt;k__BackingField").
        /// If the field is not an auto-property backing field, the name is returned unchanged.
        /// </summary>
        /// <param name="name">The raw field name, typically from reflection.</param>
        /// <returns>
        /// The property name backed by the field, or the original field name if not compiler-generated.
        /// </returns>
        public static string GetLogicalName(string name)
        {
            if (name.Length > 0 && name[0] == '<')
            {
                int end = name.IndexOf(">k__BackingField", StringComparison.Ordinal);

                if (end > 1)
                    name = name[1..end]; // "e.g. InterfaceB1"
            }

            return name;
        }
    }
}