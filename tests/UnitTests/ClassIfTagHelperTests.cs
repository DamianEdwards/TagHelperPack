using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Collections.Generic;
using System.Threading.Tasks;
using TagHelperPack;
using Xunit;

namespace UnitTests;

public class ClassIfTagHelperTests
{
    private readonly TagHelperAttributeList _attributeList;
    private readonly TagHelperContext _context;
    private readonly TagHelperOutput _output;
    private readonly ClassIfTagHelper _tagHelper;

    public  ClassIfTagHelperTests()
    {
        _attributeList = new TagHelperAttributeList();

        _context = new TagHelperContext("div", _attributeList, new Dictionary<object, object>(), "unqiueId");

        _output = new TagHelperOutput("div", _attributeList, (_, _) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));
        _output.TagName = "div";
        _output.Content.SetHtmlContent("hello");

        _tagHelper = new ClassIfTagHelper();
    }
    [Fact]
    public void IsIntanceOf_TagHelper()
    {
        Assert.IsAssignableFrom<TagHelper>(_tagHelper);
    }

    [Theory]
    // ADD 'text-muted' to existing [class] 'm-0 p-0'
    [InlineData("m-0 p-0", "text-muted", true, "m-0 p-0 text-muted")]
    // ADD 'text-muted' to null [class]
    [InlineData(null, "text-muted", true, "text-muted")]
    // ADD 'text-muted' to empty [class]
    [InlineData("", "text-muted", true, "text-muted")]
    // ADD Duplicate 'text-muted' to existing [class]='m-0 p-0 text-muted' (should not duplicate)
    [InlineData("m-0 p-0 text-muted", "text-muted", true, "m-0 p-0 text-muted")]
    // REMOVE 'text-muted' to existing [class]='m-0 p-0 text-muted' (removed because condition is false)
    [InlineData("m-0 p-0 text-muted", "text-muted", false, "m-0 p-0")]
    // REMOVE 'text-muted' to exising [class]='m-0 p-0 text-muted' (will not do anything, text-muted does not exist in the existing class)
    [InlineData("m-0 p-0", "text-muted", false, "m-0 p-0")]
    public void Process_ExistingClassNames(string existingClassNames, string classIfAttribute, bool classIfCondition, string expected)
    {
        _attributeList.Add("class", existingClassNames);

        _tagHelper.ClassIfAttributes.Add(classIfAttribute, classIfCondition);

        _tagHelper.Process(_context, _output);

        var actual = (string)_output.Attributes["class"].Value;

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("m-0 p-0", "col-2", true, "m-0 p-0 col-2")]   // with number
    [InlineData("m-0 p-0", "kebab-case", true, "m-0 p-0 kebab-case")]   // keba-case
    [InlineData("m-0 p-0", "under_score", true, "m-0 p-0 under_score")]   // underscore
    [InlineData("m-0 p-0", "_under_score", true, "m-0 p-0 _under_score")]   // underscore first
    [InlineData("m-0 p-0", "__under_score", true, "m-0 p-0 __under_score")]   // double underscore first
    [InlineData("m-0 p-0", "camelCase", true, "m-0 p-0 camelCase")]   // camelCase
    [InlineData("m-0 p-0", "PascalCase", true, "m-0 p-0 PascalCase")]   // PascalCase
    [InlineData("m-0 p-0", "kebab-case-under_score", true, "m-0 p-0 kebab-case-under_score")]   // kebab-case + underscore
    public void Process_VariousClassNames(string existingClassNames, string classIfAttribute, bool classIfCondition, string expected)
    {
        _attributeList.Add("class", existingClassNames);

        _tagHelper.ClassIfAttributes.Add(classIfAttribute, classIfCondition);

        _tagHelper.Process(_context, _output);

        var actual = (string)_output.Attributes["class"].Value;

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Process_Multiple_Entries()
    {
        _attributeList.Add("class", "m-0 p-0");

        _tagHelper.ClassIfAttributes.Add("one", true);
        _tagHelper.ClassIfAttributes.Add("one-false", false);
        _tagHelper.ClassIfAttributes.Add("two", true);
        _tagHelper.ClassIfAttributes.Add("two-false", false);
        _tagHelper.ClassIfAttributes.Add("three", true);
        _tagHelper.ClassIfAttributes.Add("three-false", false);

        _tagHelper.Process(_context, _output);

        var actual = (string)_output.Attributes["class"].Value;

        Assert.Equal("m-0 p-0 one two three", actual);
    }

    [Theory]
    [InlineData("m-0 p-0", "m-0 p-0 one ONe OnE")]
    [InlineData("m-0 p-0 one two", "m-0 p-0 one two ONe OnE")]
    public void Process_Multiple_Entries_CaseSensitive(string existing, string expected)
    {
        _attributeList.Add("class", existing);

        _tagHelper.ClassIfAttributes.Add("one", true);
        _tagHelper.ClassIfAttributes.Add("ONe", true);
        _tagHelper.ClassIfAttributes.Add("OnE", true);

        _tagHelper.Process(_context, _output);

        var actual = (string)_output.Attributes["class"].Value;

        Assert.Equal(expected, actual);
    }
}
