using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TagHelperPack
{
    /// <summary>
    /// <see cref="ITagHelper"/> implementation targeting &lt;datalist&gt; elements with <c>asp-list</c> attribute(s).
    /// </summary>
    [HtmlTargetElement("datalist", Attributes = ListAttributeName)]
    public class DatalistTagHelper : TagHelper
    {
        private const string ListAttributeName = "asp-list";

        /// <inheritdoc />
        public override int Order => -1000;

        /// <summary>
        /// A collection of <see cref="string"/> objects used to populate the &lt;datalist&gt; element with
        /// &lt;option&gt; elements.
        /// </summary>
        [HtmlAttributeName(ListAttributeName)]
        public IEnumerable<string> List { get; set; }

        /// <inheritdoc />
        /// <remarks>Does nothing if <see cref="List"/> is <c>null</c>.</remarks>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            if (List == null)
            {
                return;
            }

            var tagBuilder = GenerateDatalist(datalistList: List);

            if (tagBuilder != null && tagBuilder.HasInnerHtml)
            {
                output.PostContent.AppendHtml(tagBuilder.InnerHtml);
            }
        }

        /// <summary>
        /// Generates a &lt;datalist&gt; element with the <paramref name="datalistList"/> as &lt;option&gt;
        /// </summary>
        /// <param name="datalistList">
        /// A collection of <see cref="string"/> objects used to populate the &lt;datalist&gt; element with
        /// &lt;option&gt; elements.
        /// </param>
        /// <remarks>Should be added to <see cref="IHtmlGenerator"/>.</remarks>
        /// <returns>A new <see cref="TagBuilder"/> describing the &lt;datalist&gt; element.</returns>
        private TagBuilder GenerateDatalist(IEnumerable<string> datalistList)
        {
            if (!(datalistList is IList<string> stringList))
            {
                stringList = datalistList.ToList();
            }

            if (stringList.Count == 0)
            {
                return null;
            }

            var tagBuilder = new TagBuilder("datalist");
            var listItemBuilder = new HtmlContentBuilder(stringList.Count);
            foreach (var item in stringList)
            {
                var optionBuilder = new TagBuilder("option");
                optionBuilder.Attributes["value"] = item;
                listItemBuilder.AppendLine(optionBuilder);
            }
            tagBuilder.InnerHtml.SetHtmlContent(listItemBuilder);
            return tagBuilder;
        }
    }
}
