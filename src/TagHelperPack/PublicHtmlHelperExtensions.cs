using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TagHelperPack;

namespace Microsoft.AspNetCore.Mvc.ViewFeatures;

public static class PublicHtmlHelperExtensions
{
    private static IDictionary<string, string> htmlAttributesWithSeparators = new Dictionary<string, string>()
        {
            { "class", " "},
            { "style", "; " },
        };

    /// <summary>
    /// Merge values from 2 anonymous or IDictionary objects. Values of overlapping keys from the 'existing values' object are replaced by the values 
    /// from the 'new values' object except if the keys are 'class' or 'style', in which case the values are concatentated with a space or ; respectively.
    /// </summary>
    /// <param name="newHtmlAttributesObject">new values</param>
    /// <param name="existingHtmlAttributesObject">existing values</param>
    /// <returns></returns>
    public static IDictionary<string, object> MergeHtmlAttributesObjects(this IHtmlHelper helper, object newHtmlAttributesObject, object existingHtmlAttributesObject)
    {
        IDictionary<string, object> newHtmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(newHtmlAttributesObject);
        IDictionary<string, object> existingHtmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(existingHtmlAttributesObject);

        foreach (var item in newHtmlAttributes)
        {
            htmlAttributesWithSeparators.TryGetValue(item.Key, out string separator);

            existingHtmlAttributes.TryGetValue(item.Key, out object? value);

            if(item.Value != null)
            {
                existingHtmlAttributes[item.Key] = value != null && !string.IsNullOrEmpty(separator) ?
                    string.Format("{0}{1}{2}", existingHtmlAttributes[item.Key], separator, item.Value)
                    : item.Value;
            }
        }

        return existingHtmlAttributes;
    }
}
