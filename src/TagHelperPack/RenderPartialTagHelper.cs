using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
#if NET6_0_OR_GREATER
using Microsoft.AspNetCore.Mvc.ViewFeatures.Buffers;
#else
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
#endif
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace TagHelperPack;

/// <summary>
/// 
/// </summary>
[HtmlTargetElement("render-partial")]
public class RenderPartialTagHelper : TagHelper
{
    private const string ForAttributeName = "for";
    private const string ModelAttributeName = "model";
#if NET6_0_OR_GREATER
    private const string FallbackAttributeName = "fallback-name";
    private const string OptionalAttributeName = "optional";
#endif

    private readonly PartialTagHelper _partialTagHelper;

    /// <summary>
    /// Creates a new <see cref="RenderPartialTagHelper"/>.
    /// </summary>
    /// <param name="viewEngine">The <see cref="ICompositeViewEngine"/> used to locate the partial view.</param>
    /// <param name="viewBufferScope">The <see cref="IViewBufferScope"/>.</param>
    public RenderPartialTagHelper(ICompositeViewEngine viewEngine, IViewBufferScope viewBufferScope)
    {
        _partialTagHelper = new(viewEngine, viewBufferScope);
    }

    /// <inheritdoc />
    public override int Order => int.MaxValue; // Must run later than the IfTagHelper so that it can read the value it set.

    /// <summary>
    /// The name or path of the partial view that is rendered to the response.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// An expression to be evaluated against the current model. Cannot be used together with <see cref="Model"/>.
    /// </summary>
    [HtmlAttributeName(ForAttributeName)]
    public ModelExpression For { get; set; }

    /// <summary>
    /// The model to pass into the partial view. Cannot be used together with <see cref="For"/>.
    /// </summary>
    [HtmlAttributeName(ModelAttributeName)]
    public object Model { get; set; }

#if NET6_0_OR_GREATER
    /// <summary>
    /// When optional, executing the tag helper will no-op if the view cannot be located.
    /// Otherwise will throw stating the view could not be found.
    /// </summary>
    [HtmlAttributeName(OptionalAttributeName)]
    public bool Optional { get; set; }

    /// <summary>
    /// View to lookup if the view specified by <see cref="Name"/> cannot be located.
    /// </summary>
    [HtmlAttributeName(FallbackAttributeName)]
    public string FallbackName { get; set; }
#endif

    /// <summary>
    /// A <see cref="ViewDataDictionary"/> to pass into the partial view.
    /// </summary>
    public ViewDataDictionary ViewData { get; set; }

    /// <summary>
    /// Gets the <see cref="Microsoft.AspNetCore.Mvc.Rendering.ViewContext"/> of the executing view.
    /// </summary>
    [HtmlAttributeNotBound]
    [ViewContext]
    public ViewContext ViewContext { get; set; }

    /// <inheritdoc />
    public override void Init(TagHelperContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (Name is not null)
        {
            _partialTagHelper.Name = Name;
        }
        if (For is not null)
        {
            _partialTagHelper.For = For;
        }
        if (Model is not null)
        {
            _partialTagHelper.Model = Model;
        }
#if NET6_0_OR_GREATER
        _partialTagHelper.Optional = Optional;
        _partialTagHelper.FallbackName = FallbackName;
#endif
        _partialTagHelper.ViewData = ViewData;
        _partialTagHelper.ViewContext = ViewContext;

        _partialTagHelper.Init(context);
    }

    /// <inheritdoc />
    public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
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
            // The asp-if Tag Helper has run already and determined this element shouldn't be rendered so just return now.
            output.SuppressOutput();
            return Task.CompletedTask;
        }

        return _partialTagHelper.ProcessAsync(context, output);
    }
}
