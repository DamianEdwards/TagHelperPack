using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TagHelperPack
{
    [HtmlTargetElement("markdown")]
    public class MarkdownTagHelper : TagHelper
    {
        // TODO: Support a 'src' attribute for specifying a source markdown file

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var markdown = await output.GetChildContentAsync();
            var html = Markdig.Markdown.ToHtml(markdown.GetContent(NullHtmlEncoder.Default));
            output.Content.SetHtmlContent(html);
            output.TagName = null;
        }
    }
}
