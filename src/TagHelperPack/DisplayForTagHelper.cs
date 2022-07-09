using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TagHelperPack
{
    /// <summary>
    /// Appends the HTML markup from a display template for the specified model expression.
    /// </summary>
    [HtmlTargetElement("*", Attributes = "asp-display-for")]
    public class DisplayForTagHelper : TagHelper
    {
        private readonly IHtmlHelper _htmlHelper;

        /// <summary>
        /// Creates a new instance of the <see cref="DisplayForTagHelper" /> class.
        /// </summary>
        /// <param name="htmlHelper">The <see cref="IHtmlHelper"/>.</param>
        public DisplayForTagHelper(IHtmlHelper htmlHelper)
        {
            _htmlHelper = htmlHelper;
        }

        /// <summary>
        /// An expression to be evaluated against the current model.
        /// </summary>
        [HtmlAttributeName("asp-display-for")]
        public ModelExpression For { get; set; }

        /// <summary>
        /// The name of the template to use instead of the default one.
        /// </summary>
        [HtmlAttributeName("asp-template-name")]
        public string TemplateName { get; set; }

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

            output.PostContent.AppendHtml(_htmlHelper.Display(For, TemplateName));
        }
    }
}
