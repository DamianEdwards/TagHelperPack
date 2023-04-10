using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagHelperPack;
using Xunit;

namespace UnitTests
{
    public class EnabledTagHelperTests
    {
        [Fact]
        public void IsIntanceOf_TagHelper()
        {
            var helper = new EnabledTagHelper();

            Assert.IsAssignableFrom<TagHelper>(helper);
        }

        [Fact]
        public void IsEnabled_True_ByDefault()
        {
            var helper = new EnabledTagHelper();

            Assert.True(helper.IsEnabled);
        }

        [Fact]
        public void Process_AppendDisableAttribute_IfDisabled()
        {
            var list = new TagHelperAttributeList();

            var context = new TagHelperContext("button", list, new Dictionary<object, object>(), "unqiueId");

            var output = new TagHelperOutput("button", list, (_, _) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));
            output.TagName = "button";
            output.Content.SetHtmlContent("hello");

            var helper = new EnabledTagHelper();
            helper.IsEnabled = false;

            helper.Process(context, output);

            Assert.Equal("button", output.TagName);
            Assert.True(output.Attributes.ContainsName("disabled"));
        }

        [Fact]
        public void Process_WillNotAppendDisableAttribute_IfEnabled()
        {
            var list = new TagHelperAttributeList();

            var context = new TagHelperContext("button", list, new Dictionary<object, object>(), "unqiueId");

            var output = new TagHelperOutput("button", list, (_, _) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));
            output.TagName = "button";
            output.Content.SetHtmlContent("hello");

            var helper = new EnabledTagHelper();
            helper.IsEnabled = true;

            helper.Process(context, output);

            Assert.Equal("button", output.TagName);
            Assert.False(output.Attributes.ContainsName("disabled"));
        }
    }
}
