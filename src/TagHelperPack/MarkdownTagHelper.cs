#nullable enable

using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Ganss.XSS;
using Markdig;
using Markdig.Helpers;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;

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
        /// Makes use of Ganss.XSS.HtmlSanitizer to sanitize HTML.
        /// </remarks>
        [HtmlAttributeName("allow-html")]
        public bool AllowHtml { get; set; }

        /// <summary>
        /// When set to <c>true</c> (default), leaves the content alone in terms of indentation. When set to
        /// <c>false</c> (default) strips leading white space based on the first line of non-empty content. The
        /// first line of content determines the format of the white spacing and removes it from all other lines.
        /// </summary>
        /// <remarks>
        /// <para>
        /// When pulling markdown from a variable, to get full fidelity, you may want to do this:
        /// &lt;markdown normalize-indentation="false"&gt;@yourMarkdown&lt;/markdown&gt; otherwise cases where the markdown
        /// you're rendering starts with an indented code block will not render correctly.
        /// </para>
        /// <para>
        /// This approach is adapted from
        /// <see href="https://github.com/RickStrahl/Westwind.AspNetCore.Markdown">Westwind.AspNetCore.Markdown</see>
        /// (MIT license).
        /// </para>
        /// </remarks>
        [HtmlAttributeName("preserve-indentation")]
        public bool PreserveIndentation { get; set; } = true;

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var childContent = await output.GetChildContentAsync(NullHtmlEncoder.Default);
            var razorContent = childContent.GetContent(NullHtmlEncoder.Default);
            if (razorContent is not { Length: > 0 })
            {
                return;
            }

            var markdownHtmlContent = new MarkdownHtmlContent(razorContent, AllowHtml, PreserveIndentation);
            output.Content.SetHtmlContent(markdownHtmlContent);
            output.TagName = null;
        }

        class MarkdownHtmlContent : IHtmlContent
        {
            readonly string _content;
            readonly bool _allowHtml;
            readonly bool _preserveIndentation;

            public MarkdownHtmlContent(string content, bool allowHtml, bool preserveIndentation)
            {
                _content = content;
                _allowHtml = allowHtml;
                _preserveIndentation = preserveIndentation;
            }

            public void WriteTo(TextWriter writer, HtmlEncoder encoder)
            {
                var markdown = !_preserveIndentation ? NormalizeIndentation(_content) : _content;

                if (!_allowHtml)
                {
                    // Convert the markdown to HTML directly to the writer. This is a bit more memory efficient than
                    // converting the markdown to HTML in memory and then writing out the HTML.
                    var pipeline = new MarkdownPipelineBuilder().DisableHtml().Build();
                    Markdown.ToHtml(markdown, writer, pipeline);
                }
                else
                {
                    WriteSanitizedHtml(markdown, writer);
                }
            }

            void WriteSanitizedHtml(string markdown, TextWriter writer)
            {
                var pipeline = new MarkdownPipelineBuilder().Build();
                // I wasn't able to find a way to sanitize the HTML in a "streaming" manner so
                // we convert the markdown to HTML in memory so that we can sanitize it.
                var html = Markdown.ToHtml(markdown, pipeline);
                var sanitizedHtml = _allowHtml ? SanitizeHtml(html) : html;
                writer.Write(sanitizedHtml);
            }

            /// <summary>
            /// Strips leading white space based on shortest indented non-empty line.
            /// </summary>
            /// <remarks>
            /// The reason this method exists is indented markdown is considered to be preformatted code blocks. And the
            /// the markdown this tag helper receives might all be indented because of where the markdown tag helper is used
            /// within the page (such as indented within a div).
            /// </remarks>
            static string NormalizeIndentation(string text)
            {
                if (text is { Length: 0 } || !text[0].IsWhitespace())
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
}
