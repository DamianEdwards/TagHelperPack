#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Ganss.XSS;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Html;
using Markdig;
using Markdig.Extensions.Abbreviations;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace TagHelperPack
{
    /// <summary>
    /// Renders markdown.
    /// </summary>
    [HtmlTargetElement("markdown", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class MarkdownTagHelper : TagHelper
    {
        // TODO: Support a 'src' attribute for specifying a source markdown file

        /// <summary>
        /// When <c>true</c>, allows sanitized HTML to be rendered.
        /// </summary>
        /// <remarks>
        /// Makes use of
        /// </remarks>
        [HtmlAttributeName("allow-html")]
        public bool AllowHtml { get; set; }

        /// <summary>
        /// When set to true (default) strips leading white space based on the first line of non-empty content. The
        /// first line of content determines the format of the white spacing and removes q`it from all other lines.
        /// </summary>
        /// <remarks>
        /// <para>
        /// When pulling markdown from a variable, to get full fidelity, you may want to do this:
        /// &lt;markdown normalize-whitespace="false"&gt;@yourMarkdown&lt;/markdown&gt; otherwise cases where the markdown
        /// you're rendering starts with an indented code block will not render correctly.
        /// </para>
        /// <para>
        /// This approach is adapted from
        /// <see href="https://github.com/RickStrahl/Westwind.AspNetCore.Markdown">Westwind.AspNetCore.Markdown</see>
        /// (MIT license).
        /// </para>
        /// </remarks>
        [HtmlAttributeName("normalize-whitespace")]
        public bool NormalizeWhitespace { get; set; } = true;

>>>>>>> 0ba9182 (Implement Html support for MarkdownTagHelper)
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var childContent = await output.GetChildContentAsync(NullHtmlEncoder.Default);
            var razorContent = childContent.GetContent(NullHtmlEncoder.Default);
            if (razorContent is not { Length: > 0 })
            {
                return;
            }
            var markdown = NormalizeWhitespace ? NormalizeWhiteSpaceText(razorContent) : razorContent;

            var pipelineBuilder = new MarkdownPipelineBuilder();
            if (!AllowHtml)
            {
                pipelineBuilder = pipelineBuilder.DisableHtml();
            }
            var pipeline = pipelineBuilder.Build();
            var html = Markdown.ToHtml(markdown, pipeline);
            var sanitizedHtml = SanitizeHtml(html);

            output.Content.SetHtmlContent(sanitizedHtml);
            output.TagName = null;
            await base.ProcessAsync(context, output);
        }

        /// <summary>
        /// Strips leading white space based on shortest indented non-empty line.
        /// </summary>
        /// <remarks>
        /// The reason this method exists is indented markdown is considered to be preformatted code blocks. And the
        /// the markdown this tag helper receives might all be indented because of where the markdown tag helper is used
        /// within the page (such as indented within a div).
        /// </remarks>
        static string NormalizeWhiteSpaceText(string text)
        {
            if (text is { Length: 0 })
            {
                return text;
            }

            var lineReader = new StringReader(text);
            // In the case of something like this, the first line will be empty, so we need to advance to the first
            // non-empty line.
            // <markdown>
            //    @someMarkdown
            // </markdown>
            var firstLine = lineReader.ReadLine();
            while (firstLine is not { Length: > 0 })
            {
                firstLine = lineReader.ReadLine();
            }

            if (firstLine is not { Length: > 0 })
            {
                return text;
            }

            var indent = GetIndentation(firstLine); // We're going to use this to strip leading white space.

            return string.Join("\n", GetLines(lineReader, firstLine, indent));
        }

        static IEnumerable<string> GetLines(TextReader stringReader, string firstLine, int indent)
        {
            yield return firstLine.Substring(indent);

            while (stringReader.ReadLine() is { } line)
            {
                var indentation = GetIndentation(line);
                yield return indentation < indent
                    ? line
                    : line.Substring(indent);
            }
        }

        static int GetIndentation(string line)
        {
            int num = 0;
            while (num < line.Length && char.IsWhiteSpace(line[num]))
                ++num;
            return num;
        }

        static string SanitizeHtml(string markdownContent)
        {
            var sanitizer = new HtmlSanitizer();
            sanitizer.AllowedTags.Add("blockquote");
            sanitizer.AllowedTags.Add("footer");
            sanitizer.AllowedTags.Add("video");
            sanitizer.AllowedTags.Add("source");
            sanitizer.AllowedTags.Add("iframe");
            sanitizer.AllowedAttributes.Add("class");
            sanitizer.AllowedAttributes.Add("controls");

            return sanitizer.Sanitize(markdownContent);
        }
    }
}
