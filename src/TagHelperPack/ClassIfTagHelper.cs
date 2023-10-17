using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TagHelperPack;

/// <summary>
/// Add/Remove class name based on a condition. 
/// Supports different variations: 
/// asp-class-if-my-class, 
/// asp-class-if-class, 
/// asp-class-if-my_class, 
/// asp-class-if-MyClass, 
/// asp-class-if-myClass, 
/// asp-class-if-_my-class, 
/// asp-class-if-__myclass
/// </summary>
[HtmlTargetElement("*", Attributes = "asp-class-if*")]
public class ClassIfTagHelper : TagHelper
{
    private const string Space = " ";

    /// <summary>
    /// Add/Remove class name based on a condition
    /// </summary>
    [HtmlAttributeName(DictionaryAttributePrefix = "asp-class-if-")]
    public IDictionary<string, bool> ClassIfAttributes { get; set; } = new Dictionary<string, bool>();

    /// <inheritdoc />
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var existingClassNames = GetExistingClassNames(context);

        var addClassNames = ClassIfAttributes
            .Where(c => c.Value)
            .Select(c => GenerateClassName(c.Key))
            .ToList();

        var removeClassNames = ClassIfAttributes
            .Where(c => !c.Value)
            .Select(c => GenerateClassName(c.Key))
            .ToList();

        // remove where false
        var endResultClassNames = existingClassNames
            .Union(addClassNames)
            .Where(c => !removeClassNames.Contains(c));

        var endResultClassNameString = string.Join(Space, endResultClassNames);

        output.Attributes.SetAttribute("class", endResultClassNameString);
    }

    private static HashSet<string> GetExistingClassNames(TagHelperContext context)
    {
        var existingClassAttribute = context.AllAttributes["class"];
        if (existingClassAttribute?.Value != null)
        {
            var existingClasses = existingClassAttribute.Value.ToString()!
                .Split(Space.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            return new HashSet<string>(existingClasses);
        }

        return new HashSet<string>();
    }

    private static string GenerateClassName(string className)
    {
        // Handle className transformation here
        // for something that we hadn't consider
        return className;
    }
}
