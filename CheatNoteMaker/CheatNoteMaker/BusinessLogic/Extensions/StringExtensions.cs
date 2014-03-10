using System.Text.RegularExpressions;

namespace CheatNoteMaker.BusinessLogic.Extensions
{
    public static class StringExtensions
    {
        public static string Shorten(this string value, int length)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            var shortenedLength = length < value.Length ? length : value.Length;

            if (shortenedLength == value.Length)
            {
                return value.Substring(0, shortenedLength);
            }
            else
            {
                return value.Substring(0, shortenedLength - 1) + "…";
            }
        }

        public static string HtmlEncode(this string value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : System.Web.HttpUtility.HtmlEncode(value).Replace("\n", "<br />");
        }

        public static string AntiCodeInjection(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            var regex = new Regex("(\\<script(.+?)\\</script\\>)", RegexOptions.Singleline | RegexOptions.IgnoreCase);

            return regex.Replace(value, "");
        }
    }
}