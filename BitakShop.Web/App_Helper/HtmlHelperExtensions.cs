using System;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace BitakShop.Web
{
    public static class HtmlHelperExtensions
    {


        /// <summary>
        ///     Compares the requested route with the given <paramref name="value" /> value, if a match is found the
        ///     <paramref name="attribute" /> value is returned.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="value">The action value to compare to the requested route action.</param>
        /// <param name="attribute">The attribute value to return in the current action matches the given action value.</param>
        /// <returns>A HtmlString containing the given attribute value; otherwise an empty string.</returns>
        public static IHtmlString RouteIf(this HtmlHelper helper, string controller, string action, string attribute)
        {
            var currentController =
                (helper.ViewContext.RequestContext.RouteData.Values["controller"] ?? string.Empty).ToString();//.UnDash();
            var currentAction =
                (helper.ViewContext.RequestContext.RouteData.Values["action"] ?? string.Empty).ToString();//.UnDash();

            var hasController = controller.Equals(currentController, StringComparison.InvariantCultureIgnoreCase);
            var hasAction = action.Equals(currentAction, StringComparison.InvariantCultureIgnoreCase);

            return hasAction && hasController ? new HtmlString(attribute) : new HtmlString(string.Empty);
        }


    }
}