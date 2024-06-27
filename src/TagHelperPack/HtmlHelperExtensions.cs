using System;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Html;
using TagHelperPack;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Routing;

namespace Microsoft.AspNetCore.Mvc.ViewFeatures;

internal static class HtmlHelperExtensions
{
    private static Func<HtmlHelper, ModelExplorer, string, string> _getDisplayNameThunk;

    public static string GetExpressionText(string expression)
    {
        // If it's exactly "model", then give them an empty string, to replicate the lambda behavior.
        return string.Equals(expression, "model", StringComparison.OrdinalIgnoreCase) ? string.Empty : expression;
    }

    public static string DisplayName(this IHtmlHelper htmlHelper, ModelExpression modelExpression)
    {
        var expression = GetExpressionText(modelExpression.Name);

        if (htmlHelper is IModelHtmlHelper modelHtmlHelper)
        {
            return modelHtmlHelper.GenerateDisplayName(modelExpression.ModelExplorer, expression);
        }

        if (htmlHelper is HtmlHelper htmlHelperConcrete)
        {
            if (_getDisplayNameThunk == null)
            {
                // class HtmlHelper { protected virtual string GenerateDisplayName(ModelExplorer modelExplorer, string expression)
                var methodInfo = typeof(HtmlHelper).GetTypeInfo().GetMethod("GenerateDisplayName", BindingFlags.NonPublic | BindingFlags.Instance);
                _getDisplayNameThunk = (Func<HtmlHelper, ModelExplorer, string, string>)methodInfo.CreateDelegate(typeof(Func<HtmlHelper, ModelExplorer, string, string>));
            }

            return _getDisplayNameThunk.Invoke(htmlHelperConcrete, modelExpression.ModelExplorer, expression);
        }

        return htmlHelper.DisplayName(expression);
    }

    private static Func<HtmlHelper, ModelExplorer, string, string, object, IHtmlContent> _getDisplayThunk;

    public static IHtmlContent Display(this IHtmlHelper htmlHelper, ModelExpression modelExpression, string htmlFieldName = null, string templateName = null, object additionalViewData = null)
    {
        var expression = GetExpressionText(modelExpression.Name);

        if (htmlHelper is IModelHtmlHelper modelHtmlHelper)
        {
            return modelHtmlHelper.GenerateDisplay(modelExpression.ModelExplorer,
                htmlFieldName ?? expression, templateName, additionalViewData);
        }

        if (htmlHelper is HtmlHelper htmlHelperConcrete)
        {
            if (_getDisplayThunk == null)
            {
                // class HtmlHelper { protected virtual IHtmlContent GenerateDisplay(ModelExplorer modelExplorer, string htmlFieldName, string templateName, object additionalViewData)
                var methodInfo = typeof(HtmlHelper).GetTypeInfo().GetMethod("GenerateDisplay", BindingFlags.NonPublic | BindingFlags.Instance);
                _getDisplayThunk = (Func<HtmlHelper, ModelExplorer, string, string, object, IHtmlContent>)methodInfo.CreateDelegate(typeof(Func<HtmlHelper, ModelExplorer, string, string, object, IHtmlContent>));
            }

            return _getDisplayThunk(htmlHelperConcrete, modelExpression.ModelExplorer,
                htmlFieldName ?? expression, templateName, additionalViewData);
        }

        return htmlHelper.Display(expression, templateName, htmlFieldName, additionalViewData);
    }

    private static Func<HtmlHelper, ModelExplorer, string, string, object, IHtmlContent> _editorThunk;

    public static IHtmlContent Editor(this IHtmlHelper htmlHelper, ModelExpression modelExpression, string htmlFieldName = null, string templateName = null, object additionalViewData = null)
    {
        var expression = GetExpressionText(modelExpression.Name);

        if (htmlHelper is IModelHtmlHelper modelHtmlHelper)
        {
            return modelHtmlHelper.GenerateEditor(modelExpression.ModelExplorer,
                htmlFieldName ?? expression, templateName, additionalViewData);
        }

        if (htmlHelper is HtmlHelper htmlHelperConcrete)
        {
            if (_editorThunk == null)
            {
                // class HtmlHelper { protected virtual IHtmlContent GenerateEditor(ModelExplorer modelExplorer, string htmlFieldName, string templateName, object additionalViewData)
                var methodInfo = typeof(HtmlHelper).GetTypeInfo().GetMethod("GenerateEditor", BindingFlags.NonPublic | BindingFlags.Instance);
                _editorThunk = (Func<HtmlHelper, ModelExplorer, string, string, object, IHtmlContent>)methodInfo.CreateDelegate(typeof(Func<HtmlHelper, ModelExplorer, string, string, object, IHtmlContent>));
            }

            return _editorThunk(htmlHelperConcrete, modelExpression.ModelExplorer,
                htmlFieldName ?? expression, templateName, additionalViewData);
        }

        return htmlHelper.Editor(expression, templateName, htmlFieldName, additionalViewData);
    }

    /// <summary>
    /// Merge values from 2 anonymous or IDictionary objects. Values of overlapping keys from the 'existing values' object are replaced by the values 
    /// from the 'new values' object except if the keys are 'class' or 'style', in which case the values are concatentated with a space or ; respectively.
    /// </summary>
    /// <param name="newHtmlAttributesObject">new values</param>
    /// <param name="existingHtmlAttributesObject">existing values</param>
    /// <returns></returns>
    internal static IDictionary<string, object> MergeHtmlAttributes(this IHtmlHelper helper, object newHtmlAttributesObject, object existingHtmlAttributesObject)
    {
        var keysConcatValuesWithSpace = new string[] { "class" };
        var keysConcatValuesWithSemiColon = new string[] { "style" };

        var htmlAttributesDict = newHtmlAttributesObject as IDictionary<string, object>;
        var defaultHtmlAttributesDict = existingHtmlAttributesObject as IDictionary<string, object>;

        IDictionary<string, object> htmlAttributes = (htmlAttributesDict != null)
            ? new RouteValueDictionary(htmlAttributesDict)
            : HtmlHelper.AnonymousObjectToHtmlAttributes(newHtmlAttributesObject);

        IDictionary<string, object> existingHtmlAttributes = (defaultHtmlAttributesDict != null)
            ? new RouteValueDictionary(defaultHtmlAttributesDict)
            : HtmlHelper.AnonymousObjectToHtmlAttributes(existingHtmlAttributesObject);

        foreach (var item in htmlAttributes)
        {
            if (keysConcatValuesWithSpace.Contains(item.Key))
            {
                existingHtmlAttributes.TryGetValue(item.Key, out object? value);
                if (value != null && item.Value != null)
                {
                    existingHtmlAttributes[item.Key] = value != null ?
                        string.Format("{0} {1}", existingHtmlAttributes[item.Key], item.Value)
                        : item.Value;
                }
            }
            else if (keysConcatValuesWithSemiColon.Contains(item.Key))
            {
                existingHtmlAttributes.TryGetValue(item.Key, out object? value);
                if (value != null && item.Value != null)
                {
                    existingHtmlAttributes[item.Key] = value != null ?
                        string.Format("{0}; {1}", existingHtmlAttributes[item.Key], item.Value)
                        : item.Value;
                }
            }
            else
            {
                existingHtmlAttributes[item.Key] = item.Value;
            }
        }

        return existingHtmlAttributes;
    }
}
