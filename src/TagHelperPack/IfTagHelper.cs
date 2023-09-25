using System;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace TagHelperPack;

/// <summary>
/// Suppresses the output of the element if the supplied predicate equates to <c>false</c>.
/// </summary>
[HtmlTargetElement("*", Attributes = "asp-if")]
public class IfTagHelper : TagHelper
{
    internal static object SuppressedKey = new();
    internal static object SuppressedValue = new();

    /// <summary>
    /// Gets or sets the predicate expression to test.
    /// </summary>
    [HtmlAttributeName("asp-if")]
    public bool Predicate { get; set; }

    /// <inheritdoc />
    public override int Order => - 1;

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

        if (!Predicate)
        {
            output.SuppressOutput();
            context.Items[SuppressedKey] = SuppressedValue;
        }
    }
}

/// <summary>
/// Extension methods for <see cref="TagHelperContext"/>.
/// </summary>
public static class TagHelperContextExtensions
{
    /// <summary>
    /// Determines if the <see cref="IfTagHelper"/> (<c>asp-if</c>) has suppressed rendering for the element associated with
    /// this <see cref="TagHelperContext"/>.
    /// </summary>
    /// <param name="context">The <see cref="TagHelperContext"/>.</param>
    /// <returns><c>true</c> if <c>asp-if</c> evaluated to <c>false</c>, else <c>false</c>.</returns>
    public static bool SuppressedByAspIf(this TagHelperContext context) =>
        context.Items.TryGetValue(IfTagHelper.SuppressedKey, out var value) && value == IfTagHelper.SuppressedValue;
}
