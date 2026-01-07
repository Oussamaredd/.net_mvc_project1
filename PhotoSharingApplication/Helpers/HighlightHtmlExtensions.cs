using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace PhotoSharingApplication.Helpers
{
    public static class HighlightHtmlExtensions
    {
        public static IHtmlString Highlight(this HtmlHelper html, string text, string term)
        {
            if (string.IsNullOrEmpty(text))
                return new HtmlString(string.Empty);

            if (string.IsNullOrWhiteSpace(term))
                return new HtmlString(HttpUtility.HtmlEncode(text));

            term = (term ?? "").Trim();

            if (term.Length == 0)
                return new HtmlString(HttpUtility.HtmlEncode(text));

            if (term.Length > 80)
                term = term.Substring(0, 80);

            var pattern = Regex.Escape(term);
            var regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

            var sb = new StringBuilder();
            int lastIndex = 0;

            foreach (Match m in regex.Matches(text))
            {
                // Encode the non-matching part
                var before = text.Substring(lastIndex, m.Index - lastIndex);
                sb.Append(HttpUtility.HtmlEncode(before));

                // Encode the matching part and wrap it
                var matchText = text.Substring(m.Index, m.Length);
                sb.Append("<mark class=\"px-1 rounded highlight-term\">");

                sb.Append(HttpUtility.HtmlEncode(matchText));
                sb.Append("</mark>");

                lastIndex = m.Index + m.Length;
            }

            // Encode remaining tail
            sb.Append(HttpUtility.HtmlEncode(text.Substring(lastIndex)));

            return new HtmlString(sb.ToString());
        }
    }
}
