using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TagHelperPack
{
    /// <summary>
    /// Renders the HTML markup from a display template for the specified model expression.
    /// </summary>
    [HtmlTargetElement("display", Attributes = "for", TagStructure = TagStructure.WithoutEndTag)]
    public class DisplayTagHelper : TagHelper
    {
        private readonly IHtmlHelper _htmlHelper;

        public DisplayTagHelper(IHtmlHelper htmlHelper)
        {
            _htmlHelper = htmlHelper;
        }

        /// <summary>
        /// An expression to be evaluated against the current model.
        /// </summary>
        [HtmlAttributeName("for")]
        public ModelExpression For { get; set; }

        /// <summary>
        /// The name of the template to use instead of the default one.
        /// </summary>
        [HtmlAttributeName("template-name")]
        public string TemplateName { get; set; }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            ((IViewContextAware)_htmlHelper).Contextualize(ViewContext);

            output.Content.SetHtmlContent(_htmlHelper.Display(For, TemplateName));

            output.TagName = null;
        }
    }
}
