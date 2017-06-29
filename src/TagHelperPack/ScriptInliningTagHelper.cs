using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.FileProviders;

namespace TagHelperPack
{
    /// <summary>
    /// Allows inlining the content of the referenced JavaScript file into the element body.
    /// </summary>
    [HtmlTargetElement("script", Attributes = "inline")]
    public class ScriptInliningTagHelper : TagHelper
    {
        private readonly IFileProvider _wwwroot;

        public ScriptInliningTagHelper(IHostingEnvironment env)
        {
            _wwwroot = env.WebRootFileProvider;
        }

        /// <summary>
        /// Specifies whether the script file should be inlined.
        /// </summary>
        public bool Inline { get; set; }

        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!Inline)
            {
                return;
            }

            var src = output.Attributes["src"];

            if (src == null)
            {
                return;
            }

            var path = default(string);
            switch (src.Value)
            {
                case string p:
                    path = p;
                    break;
                case HtmlString s when s.Value != null:
                    path = s.Value;
                    break;
                case IHtmlContent pathHtmlContent:
                    using (var tw = new StringWriter())
                    {
                        pathHtmlContent.WriteTo(tw, NullHtmlEncoder.Default);
                        path = tw.ToString();
                    }
                    break;
                default:
                    path = src?.Value?.ToString();
                    break;
            }
            var resolvedPath = path ?? src.Value.ToString();

            var queryStringStartIndex = resolvedPath.IndexOf('?');
            if (queryStringStartIndex != -1)
            {
                resolvedPath = resolvedPath.Substring(0, queryStringStartIndex);
            }

            if (Uri.TryCreate(resolvedPath, UriKind.Absolute, out Uri uri))
            {
                // Don't inline if the path is absolute
                return;
            }

            var fileInfo = _wwwroot.GetFileInfo(resolvedPath);
            var requestPathBase = ViewContext.HttpContext.Request.PathBase;
            if (!fileInfo.Exists)
            {
                if (requestPathBase.HasValue &&
                    resolvedPath.StartsWith(requestPathBase.Value, StringComparison.OrdinalIgnoreCase))
                {
                    resolvedPath = resolvedPath.Substring(requestPathBase.Value.Length);
                    fileInfo = _wwwroot.GetFileInfo(resolvedPath);
                }

                if (!fileInfo.Exists)
                {
                    // Don't inline if the file is not on the current server
                    return;
                }
            }

            using (var readStream = fileInfo.CreateReadStream())
            using (var reader = new StreamReader(readStream, Encoding.UTF8))
            {
                output.Content.AppendHtml(reader.ReadToEnd());
            }

            output.Attributes.Remove(src);
        }
    }
}
