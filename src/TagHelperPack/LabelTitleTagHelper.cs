using System;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace TagHelperPack;

/// <summary>
/// Sets the title attribute of the element to the description for the specified model property.
/// </summary>
[HtmlTargetElement("label", Attributes = "asp-for")]
public class LabelTitleTagHelper : TagHelper
{
    /// <summary>
    /// Gets or sets an expression to be evaluated against the current model.
    /// </summary>
    [HtmlAttributeName("asp-for")]
    public ModelExpression For { get; set; }

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

        var description = For.Metadata.Description;
        if (!string.IsNullOrEmpty(description) && !output.Attributes.ContainsName("title"))
        {
            output.Attributes.Add(new TagHelperAttribute("title", description));
        }
    }
}
