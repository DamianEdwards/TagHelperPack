using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TagHelperPack
{
    /// <summary>
    /// Appends the HTML markup from an editor template for the specified model expression.
    /// </summary>
    [HtmlTargetElement("*", Attributes = "asp-editor-for")]
    public class EditorForTagHelper : TagHelper
    {
        private readonly IHtmlHelper _htmlHelper;

        /// <summary>
        /// Creates a new instance of the <see cref="EditorForTagHelper" /> class.
        /// </summary>
        /// <param name="htmlHelper">The <see cref="IHtmlHelper"/>.</param>
        public EditorForTagHelper(IHtmlHelper htmlHelper)
        {
            _htmlHelper = htmlHelper;
        }

        /// <summary>
        /// An expression to be evaluated against the current model.
        /// </summary>
        [HtmlAttributeName("asp-editor-for")]
        public ModelExpression For { get; set; }

        /// <summary>
        /// The name of the HTML field to use instead of the default one.
        /// </summary>
        [HtmlAttributeName("asp-html-field-name")]
        public string HtmlFieldName { get; set; }

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

            output.PostContent.AppendHtml(_htmlHelper.Editor(For, HtmlFieldName, TemplateName));
        }
    }
}
