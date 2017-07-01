using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace TagHelperPack
{
    /// <summary>
    /// Renders markdown.
    /// </summary>
    [HtmlTargetElement("markdown")]
    public class MarkdownTagHelper : TagHelper
    {
        private static readonly IMemoryCache Cache;

        static MarkdownTagHelper()
        {
            var cacheOptions = new MemoryCacheOptions
            {
                CompactOnMemoryPressure = false
            };
            Cache = new MemoryCache(new OptionsWrapper<MemoryCacheOptions>(cacheOptions));
        }

        // TODO: Support a 'src' attribute for specifying a source markdown file

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var cacheKey = context.UniqueId;
            var html = await Cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                var markdown = await output.GetChildContentAsync();
                return Markdig.Markdown.ToHtml(markdown.GetContent(NullHtmlEncoder.Default));
            });

            output.Content.SetHtmlContent(html);
            output.TagName = null;
        }
    }
}
