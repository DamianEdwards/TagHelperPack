using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace TagHelperPack;

/// <summary>
/// Appends the HTML markup from an editor template for the specified model expression.
/// </summary>
[HtmlTargetElement("*", Attributes = "asp-editor-for")]
public class EditorForTagHelper : TagHelper
{
    private const string ViewDataDictionaryName = "asp-view-data";
    private const string ViewDataPrefix = "asp-view-data-";

    private readonly IHtmlHelper _htmlHelper;
    private IDictionary<string, object> _viewData;

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
    /// Additional view data.
    /// </summary>
    [HtmlAttributeName(ViewDataDictionaryName, DictionaryAttributePrefix = ViewDataPrefix)]
    public IDictionary<string, object> ViewData
    {
        get => _viewData ??= new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        set => _viewData = value;
    }

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

        if (context.SuppressedByAspIf() || context.SuppressedByAspAuthz())
        {
            return;
        }

        ((IViewContextAware)_htmlHelper).Contextualize(ViewContext);

        output.PostContent.AppendHtml(_htmlHelper.Editor(For, HtmlFieldName, TemplateName, ViewData));
    }
}
