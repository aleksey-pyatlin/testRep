using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace CheatNoteMaker.BusinessLogic.Extensions
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString Boolean(this HtmlHelper html, string name, bool value)
        {
            return html.CheckBox(name, value, new { onclick = "return false;" });
        }
    }
}