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

        /// <summary>
        /// Creates a new instance of the <see cref="DisplayNameTagHelper" /> class.
        /// </summary>
        /// <param name="htmlHelper">The <see cref="IHtmlHelper"/>.</param>
        public DisplayNameTagHelper(IHtmlHelper htmlHelper)
        {
            _htmlHelper = htmlHelper;
        }

        /// <summary>
        /// An expression to be evaluated against the current model.
        /// </summary>
        [HtmlAttributeName("for")]
        public ModelExpression For { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ViewContext"/>.
        /// </summary>
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        /// <inheritdoc />
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            ((IViewContextAware)_htmlHelper).Contextualize(ViewContext);

            output.PostContent.AppendHtml(_htmlHelper.DisplayName(For));

            output.TagName = null;
        }
    }
}
