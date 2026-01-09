using System.Text;

namespace Plugins.Saneject.Experimental.Editor.Extensions
{
    public static class StringBuilderExtensions
    {
        public static StringBuilder AppendQuantity(
            this StringBuilder stringBuilder,
            int count,
            string singular,
            string plural,
            string suffix = "",
            bool addCount = true)
        {
            if (addCount)
                stringBuilder
                    .Append(count)
                    .Append(" ");

            stringBuilder.Append(count == 1 ? singular : plural);

            if (!string.IsNullOrEmpty(suffix))
                stringBuilder.Append(suffix);

            return stringBuilder;
        }
    }
}