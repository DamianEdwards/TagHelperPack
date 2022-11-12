using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace TagHelperPack;

/// <summary>
/// Suppresses rendering of an element unless specific authorization policies are met.
/// </summary>
[HtmlTargetElement("*", Attributes = AspAuthzAttributeName)]
[HtmlTargetElement("*", Attributes = AspAuthzPolicyAttributeName)]
public class AuthzTagHelper : TagHelper
{
    private const string AspAuthzAttributeName = "asp-authz";
    private const string AspAuthzPolicyAttributeName = "asp-authz-policy";

    private readonly IAuthorizationService _authz;

    /// <summary>
    /// Creates a new instance of the <see cref="AuthzTagHelper" /> class.
    /// </summary>
    /// <param name="authz">The <see cref="IAuthorizationService"/>.</param>
    public AuthzTagHelper(IAuthorizationService authz)
    {
        _authz = authz;
    }

    /// <summary>
    /// A boolean indicating whether the current element requires authentication in order to be rendered.
    /// </summary>
    [HtmlAttributeName(AspAuthzAttributeName)]
    public bool RequiresAuthentication { get; set; }

    /// <summary>
    /// An authorization policy name that must be satisfied in order for the current element to be rendered.
    /// </summary>
    [HtmlAttributeName(AspAuthzPolicyAttributeName)]
    public string RequiredPolicy { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="ViewContext"/>.
    /// </summary>
    [HtmlAttributeNotBound]
    [ViewContext]
    public ViewContext ViewContext { get; set; }

    /// <inheritdoc />
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (output == null)
        {
            throw new ArgumentNullException(nameof(output));
        }

        if (context.SuppressedByAspIf())
        {
            return;
        }

        var requiresAuth = RequiresAuthentication || !string.IsNullOrEmpty(RequiredPolicy);
        var showOutput = false;

        var user = ViewContext.HttpContext.User;
        if (context.AllAttributes[AspAuthzAttributeName] != null && !requiresAuth && !user.Identity.IsAuthenticated)
        {
            // authz="false" & user isn't authenticated
            showOutput = true;
        }
        else if (!string.IsNullOrEmpty(RequiredPolicy))
        {
            // auth-policy="foo" & user is authorized for policy "foo"
            var cacheKey = AspAuthzPolicyAttributeName + "." + RequiredPolicy;
            bool authorized;
            var cachedResult = ViewContext.ViewData[cacheKey];
            if (cachedResult != null)
            {
                authorized = (bool)cachedResult;
            }
            else
            {
                var authResult = await _authz.AuthorizeAsync(user, RequiredPolicy);
                authorized = authResult.Succeeded;
                ViewContext.ViewData[cacheKey] = authorized;
            }

            showOutput = authorized;
        }
        else if (requiresAuth && user.Identity.IsAuthenticated)
        {
            // auth="true" & user is authenticated
            showOutput = true;
        }

        if (!showOutput)
        {
            output.SuppressOutput();
        }
    }
}