using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace TagHelperPack
{
    /// <summary>
    /// Suppresses the output of the element if the supplied predicate equates to <c>false</c>.
    /// </summary>
    [HtmlTargetElement("*", Attributes = "asp-if")]
    public class IfTagHelper : TagHelper
    {
        /// <summary>
        /// The predicate expression to test.
        /// </summary>
        [HtmlAttributeName("asp-if")]
        public bool Predicate { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!Predicate)
            {
                output.SuppressOutput();
            }
        }
    }
}
