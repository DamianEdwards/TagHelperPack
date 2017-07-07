using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TagHelperPack
{
    /// <summary>
    /// Renders the HTML markup from an editor template for the specified model expression.
    /// </summary>
    [HtmlTargetElement("editor", Attributes = "for", TagStructure = TagStructure.WithoutEndTag)]
    public class EditorTagHelper : TagHelper
    {
        private readonly IHtmlHelper _htmlHelper;

        public EditorTagHelper(IHtmlHelper htmlHelper)
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

            output.Content.SetHtmlContent(_htmlHelper.Editor(For));

            output.TagName = null;
        }
    }
}
