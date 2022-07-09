using System.IO;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Html;
using Markdig;

namespace TagHelperPack
{
    /// <summary>
    /// Renders markdown.
    /// </summary>
    [HtmlTargetElement("markdown", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class MarkdownTagHelper : TagHelper
    {
        // TODO: Support a 'src' attribute for specifying a source markdown file

        /// <inheritdoc />
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var markdownRazorContent = await output.GetChildContentAsync(NullHtmlEncoder.Default);
            var markdownHtmlContent = new MarkdownHtmlContent(markdownRazorContent.GetContent(NullHtmlEncoder.Default));

            output.Content.SetHtmlContent(markdownHtmlContent);
            output.TagName = null;
        }

        private class MarkdownHtmlContent : IHtmlContent
        {
            private readonly string _markdown;

            public MarkdownHtmlContent(string markdown)
            {
                _markdown = markdown;
            }

            public void WriteTo(TextWriter writer, HtmlEncoder encoder)
            {
                var pipeline = new MarkdownPipelineBuilder().DisableHtml().Build();
                Markdig.Markdown.ToHtml(_markdown, writer, pipeline);
            }
        }
    }
}
