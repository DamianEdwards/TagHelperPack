using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace TagHelperPack;

/// <summary>
/// Appends the display name for the specified model expression.
/// </summary>
[HtmlTargetElement("*", Attributes = "asp-display-name-for")]
public class DisplayNameForTagHelper : TagHelper
{
    private readonly IHtmlHelper _htmlHelper;

    /// <summary>
    /// Creates a new instance of the <see cref="DisplayNameForTagHelper" /> class.
    /// </summary>
    /// <param name="htmlHelper">The <see cref="IHtmlHelper"/>.</param>
    public DisplayNameForTagHelper(IHtmlHelper htmlHelper)
    {
        _htmlHelper = htmlHelper;
    }

    /// <summary>
    /// An expression to be evaluated against the current model.
    /// </summary>
    [HtmlAttributeName("asp-display-name-for")]
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
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (output == null)
        {
            throw new ArgumentNullException(nameof(output));
        }

        if (context.SuppressedByAspIf())
        {
            return;
        }

        ((IViewContextAware)_htmlHelper).Contextualize(ViewContext);

        output.PostContent.AppendHtml(_htmlHelper.DisplayName(For));
    }
}
