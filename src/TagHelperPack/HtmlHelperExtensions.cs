using System;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Html;
using TagHelperPack.Internal;

namespace Microsoft.AspNetCore.Mvc.ViewFeatures
{
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
    }
}
