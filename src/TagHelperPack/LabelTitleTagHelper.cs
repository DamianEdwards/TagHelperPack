using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace TagHelperPack
{
    /// <summary>
    /// Sets the title attribute of the element to the description for the specified model property.
    /// </summary>
    [HtmlTargetElement("label", Attributes = "asp-for")]
    public class LabelTitleTagHelper : TagHelper
    {
        /// <summary>
        /// An expression to be evaluated against the current model.
        /// </summary>
        [HtmlAttributeName("asp-for")]
        public ModelExpression For { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var description = For.Metadata.Description;
            if (!string.IsNullOrEmpty(description) && !output.Attributes.ContainsName("title"))
            {
                output.Attributes.Add(new TagHelperAttribute("title", description));
            }
        }
    }
}
