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

        /// <summary>
        /// Creates a new instance of the <see cref="EditorTagHelper" /> class.
        /// </summary>
        /// <param name="htmlHelper">The <see cref="IHtmlHelper"/>.</param>
        public EditorTagHelper(IHtmlHelper htmlHelper)
        {
            _htmlHelper = htmlHelper;
        }

        /// <summary>
        /// An expression to be evaluated against the current model.
        /// </summary>
        [HtmlAttributeName("for")]
        public ModelExpression For { get; set; }

        /// <summary>
        /// The name of the HTML field to use instead of the default one.
        /// </summary>
        [HtmlAttributeName("html-field-name")]
        public string HtmlFieldName { get; set; }

        /// <summary>
        /// The name of the template to use instead of the default one.
        /// </summary>
        [HtmlAttributeName("template-name")]
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

            output.Content.SetHtmlContent(_htmlHelper.Editor(For, HtmlFieldName, TemplateName));

            output.TagName = null;
        }
    }
}
