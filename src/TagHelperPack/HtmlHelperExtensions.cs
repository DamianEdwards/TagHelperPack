using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Html;
using System.Linq.Expressions;

namespace Microsoft.AspNetCore.Mvc.ViewFeatures
{
    public static class HtmlHelperExtensions
    {
        private static Func<HtmlHelper, ModelExplorer, string, string> _getDisplayNameThunk;

        public static string GetExpressionText(string expression)
        {
            // If it's exactly "model", then give them an empty string, to replicate the lambda behavior.
            return string.Equals(expression, "model", StringComparison.OrdinalIgnoreCase) ? string.Empty : expression;
        }

        public static string DisplayName(this IHtmlHelper htmlHelper, ModelExpression modelExpression)
        {
            if (htmlHelper is HtmlHelper htmlHelperConcrete)
            {
                if (_getDisplayNameThunk == null)
                {
                    // class HtmlHelper { protected virtual string GenerateDisplayName(ModelExplorer modelExplorer, string expression)
                    var methodInfo = typeof(HtmlHelper).GetTypeInfo().GetMethod("GenerateDisplayName", BindingFlags.NonPublic | BindingFlags.Instance);
                    _getDisplayNameThunk = (Func<HtmlHelper, ModelExplorer, string, string>)methodInfo.CreateDelegate(typeof(Func<HtmlHelper, ModelExplorer, string, string>));
                }

                return _getDisplayNameThunk.Invoke(htmlHelperConcrete, modelExpression.ModelExplorer, GetExpressionText(modelExpression.Name));
            }

            return htmlHelper.DisplayName(GetExpressionText(modelExpression.Name));
        }

        private static Func<HtmlHelper, ModelExplorer, string, string, object, IHtmlContent> _getDisplayThunk;

        public static IHtmlContent Display(this IHtmlHelper htmlHelper, ModelExpression modelExpression, string templateName = null)
        {
            if (htmlHelper is HtmlHelper htmlHelperConcrete)
            {
                if (_getDisplayThunk == null)
                {
                    // class HtmlHelper { protected virtual IHtmlContent GenerateDisplay(ModelExplorer modelExplorer, string htmlFieldName, string templateName, object additionalViewData)
                    var methodInfo = typeof(HtmlHelper).GetTypeInfo().GetMethod("GenerateDisplay", BindingFlags.NonPublic | BindingFlags.Instance);
                    _getDisplayThunk = (Func<HtmlHelper, ModelExplorer, string, string, object, IHtmlContent>)methodInfo.CreateDelegate(typeof(Func<HtmlHelper, ModelExplorer, string, string, object, IHtmlContent>));
                }

                return _getDisplayThunk(htmlHelperConcrete, modelExpression.ModelExplorer, GetExpressionText(modelExpression.Name), templateName, null);
            }

            return htmlHelper.Display(GetExpressionText(modelExpression.Name), templateName);
        }

        private static Func<HtmlHelper, ModelExplorer, string, string, object, IHtmlContent> _editorThunk;

        public static IHtmlContent Editor(this IHtmlHelper htmlHelper, ModelExpression modelExpression, string templateName = null)
        {
            if (htmlHelper is HtmlHelper htmlHelperConcrete)
            {
                if (_editorThunk == null)
                {
                    // class HtmlHelper { protected virtual IHtmlContent GenerateEditor(ModelExplorer modelExplorer, string htmlFieldName, string templateName, object additionalViewData)
                    var methodInfo = typeof(HtmlHelper).GetTypeInfo().GetMethod("GenerateEditor", BindingFlags.NonPublic | BindingFlags.Instance);
                    _editorThunk = (Func<HtmlHelper, ModelExplorer, string, string, object, IHtmlContent>)methodInfo.CreateDelegate(typeof(Func<HtmlHelper, ModelExplorer, string, string, object, IHtmlContent>));
                }

                return _editorThunk(htmlHelperConcrete, modelExpression.ModelExplorer, GetExpressionText(modelExpression.Name), templateName, null);
            }

            return htmlHelper.Editor(GetExpressionText(modelExpression.Name), templateName);
        }
    }
}
