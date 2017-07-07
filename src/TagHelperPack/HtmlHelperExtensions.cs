using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;

namespace Microsoft.AspNetCore.Mvc.ViewFeatures
{
    public static class HtmlHelperExtensions
    {
        private static Func<HtmlHelper, ModelExplorer, string, string> _getDisplayNameThunk;

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

                return _getDisplayNameThunk.Invoke(htmlHelperConcrete, modelExpression.ModelExplorer, ExpressionHelper.GetExpressionText(modelExpression.Name));
            }

            return htmlHelper.DisplayName(ExpressionHelper.GetExpressionText(modelExpression.Name));
        }

        private static Func<HtmlHelper, ModelExplorer, string, string, object, IHtmlContent> _getDisplayThunk;

        public static IHtmlContent Display(this IHtmlHelper htmlHelper, ModelExpression modelExpression)
        {
            if (htmlHelper is HtmlHelper htmlHelperConcrete)
            {
                if (_getDisplayThunk == null)
                {
                    // class HtmlHelper { protected virtual IHtmlContent GenerateDisplay(ModelExplorer modelExplorer, string htmlFieldName, string templateName, object additionalViewData)
                    var methodInfo = typeof(HtmlHelper).GetTypeInfo().GetMethod("GenerateDisplay", BindingFlags.NonPublic | BindingFlags.Instance);
                    _getDisplayThunk = (Func<HtmlHelper, ModelExplorer, string, string, object, IHtmlContent>)methodInfo.CreateDelegate(typeof(Func<HtmlHelper, ModelExplorer, string, string, object, IHtmlContent>));
                }

                return _getDisplayThunk(htmlHelperConcrete, modelExpression.ModelExplorer, ExpressionHelper.GetExpressionText(modelExpression.Name), null, null);
            }

            return htmlHelper.Display(ExpressionHelper.GetExpressionText(modelExpression.Name));
        }

        private static Func<HtmlHelper, ModelExplorer, string, string, object, IHtmlContent> _editorThunk;

        public static IHtmlContent Editor(this IHtmlHelper htmlHelper, ModelExpression modelExpression)
        {
            if (htmlHelper is HtmlHelper htmlHelperConcrete)
            {
                if (_editorThunk == null)
                {
                    // class HtmlHelper { protected virtual IHtmlContent GenerateEditor(ModelExplorer modelExplorer, string htmlFieldName, string templateName, object additionalViewData)
                    var methodInfo = typeof(HtmlHelper).GetTypeInfo().GetMethod("GenerateEditor", BindingFlags.NonPublic | BindingFlags.Instance);
                    _editorThunk = (Func<HtmlHelper, ModelExplorer, string, string, object, IHtmlContent>)methodInfo.CreateDelegate(typeof(Func<HtmlHelper, ModelExplorer, string, string, object, IHtmlContent>));
                }

                return _editorThunk(htmlHelperConcrete, modelExpression.ModelExplorer, ExpressionHelper.GetExpressionText(modelExpression.Name), null, null);
            }

            return htmlHelper.Editor(ExpressionHelper.GetExpressionText(modelExpression.Name));
        }
    }
}
