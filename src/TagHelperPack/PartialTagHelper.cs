using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace TagHelperPack
{
    /// <summary>
    /// Renders a CSHTML partial.
    /// </summary>
    [HtmlTargetElement("partial", Attributes = "name", TagStructure = TagStructure.WithoutEndTag)]
    public class PartialTagHelper : TagHelper
    {
        private readonly IHtmlHelper _htmlHelper;

        public PartialTagHelper(IHtmlHelper htmlHelper)
        {
            _htmlHelper = htmlHelper;
        }

        /// <summary>
        /// The name of the partial view used to create the HTML markup. Must not be <c>null</c>.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A model to pass into the partial view.
        /// </summary>
        public object Model { get; set; }

        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            ((IViewContextAware)_htmlHelper).Contextualize(ViewContext);

            output.TagName = null;

            var content = await (Model == null ? _htmlHelper.PartialAsync(Name) : _htmlHelper.PartialAsync(Name, Model));
            output.Content.SetHtmlContent(content);
        }
    }
}
