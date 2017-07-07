using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace TagHelperPack
{
    /// <summary>
    /// Renders the display name for the specified model expression.
    /// </summary>
    [HtmlTargetElement("display-name", Attributes = "for", TagStructure = TagStructure.WithoutEndTag)]
    public class DisplayNameTagHelper : TagHelper
    {
        private readonly IHtmlHelper _htmlHelper;

        public DisplayNameTagHelper(IHtmlHelper htmlHelper)
        {
            _htmlHelper = htmlHelper;
        }

        /// <summary>
        /// An expression to be evaluated against the current model.
        /// </summary>
        [HtmlAttributeName("for")]
        public ModelExpression For { get; set; }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            ((IViewContextAware)_htmlHelper).Contextualize(ViewContext);

            output.PostContent.AppendHtml(_htmlHelper.DisplayName(For));

            output.TagName = null;
        }
    }
}
