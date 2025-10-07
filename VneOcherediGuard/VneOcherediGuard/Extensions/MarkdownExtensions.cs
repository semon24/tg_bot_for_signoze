using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VneOcherediGuard.Extensions
{
    internal static class MarkdownExtensions
    {
        public static string? TextSanitize(string? text)
        {
            if (text is null)
            {
                return null;
            }

            return text
                .Replace(".", "\\.")
                .Replace("-", "\\-")
                .Replace("(", "\\(")
                .Replace(")", "\\)")
                .Replace("!", "\\!")
                .Replace("=", "\\=")
                .Replace("_", "\\_")
                .Replace("*", "\\*")
                .Replace("[", "\\[")
                .Replace("]", "\\]")
                .Replace("~", "\\~")
                .Replace("`", "\\`")
                .Replace(">", "\\>")
                .Replace("+", "\\+")
                .Replace("|", "\\|")
                .Replace("{", "\\{")
                .Replace("}", "\\}")
                .Replace("#", "\\#");
        }
    }
}
