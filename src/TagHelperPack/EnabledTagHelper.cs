using Microsoft.AspNetCore.Razor.TagHelpers;

namespace TagHelperPack;

/// <summary>
/// Enable/disable elements. Disabling by adding <c>disabled="disabled"</c> attribute to the element.
/// Supported elements: https://developer.mozilla.org/en-US/docs/Web/HTML/Attributes/disabled
/// </summary>
[HtmlTargetElement("button", Attributes = "[asp-enabled]")]
[HtmlTargetElement("fieldset", Attributes = "[asp-enabled]")]
[HtmlTargetElement("keygen", Attributes = "[asp-enabled]")]
[HtmlTargetElement("optgroup", Attributes = "[asp-enabled]")]
[HtmlTargetElement("option", Attributes = "[asp-enabled]")]
[HtmlTargetElement("select", Attributes = "[asp-enabled]")]
[HtmlTargetElement("textarea", Attributes = "[asp-enabled]")]
[HtmlTargetElement("input", Attributes = "[asp-enabled]")]
public class EnabledTagHelper : TagHelper
{
    /// <summary>
    /// Enable this element when the condition is <c>true</c>. Defaults to <c>true</c>.
    /// If <c>false</c>, will add <c>disabled="disabled"</c> attribute to the element.
    /// </summary>
    [HtmlAttributeName("asp-enabled")]
    public bool IsEnabled { get; set; } = true;

    /// <inheritdoc />
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (context.SuppressedByAspIf() || context.SuppressedByAspAuthz())
        {
            return;
        }

        // If it's not enabled, append disabled attribute
        if (!IsEnabled)
        {
            output.Attributes.SetAttribute("disabled", "disabled");
        }
    }
}
