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
[HtmlTargetElement("*", Attributes = AspAuthzRoleAttributeName)]
[HtmlTargetElement("*", Attributes = AspAuthzPolicyPermissionAttributeName)]
public class AuthzTagHelper : TagHelper
{
    internal static object SuppressedKey = new();
    internal static object SuppressedValue = new();

    private const string AspAuthzAttributeName = "asp-authz";
    private const string AspAuthzPolicyAttributeName = "asp-authz-policy";
    private const string AspAuthzRoleAttributeName = "asp-authz-role";
    private const string AspAuthzPolicyPermissionAttributeName = "asp-authz-permission";

    private readonly IAuthorizationService _authz;

    /// <summary>
    /// Creates a new instance of the <see cref="AuthzTagHelper" /> class.
    /// </summary>
    /// <param name="authz">The <see cref="IAuthorizationService"/>.</param>
    public AuthzTagHelper(IAuthorizationService authz)
    {
        _authz = authz;
    }

    /// <inheritdoc />
    // Run before other Tag Helpers (default Order is 0) so they can cooperatively decide not to run.
    // Note this value is coordinated with the value of IfTagHelper.Order to ensure the IfTagHelper logic runs first.
    // (Lower values run earlier).
    public override int Order => -10;

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
    /// A role that the User must belong to in order for the current element to be rendered.
    /// </summary>
    [HtmlAttributeName(AspAuthzRoleAttributeName)]
    public string RequiredRole { get; set; }

    /// <summary>
    /// A permission that must be satisfied in order for the current element to be rendered. asp-authz-policy should be set as well.
    /// </summary>
    [HtmlAttributeName(AspAuthzPolicyPermissionAttributeName)]
    public string RequiredPolicyPermission { get; set; }

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

        if (context.SuppressedByAspIf() || context.SuppressedByAspAuthz())
        {
            return;
        }

        var requiresAuth = RequiresAuthentication || !string.IsNullOrEmpty(RequiredPolicy) || !string.IsNullOrEmpty(RequiredRole);
        var showOutput = false;

        var user = ViewContext.HttpContext.User;
        if (context.AllAttributes[AspAuthzAttributeName] != null && !requiresAuth && !user.Identity.IsAuthenticated)
        {
            // authz="false" & user isn't authenticated
            showOutput = true;
        }
        else if (!string.IsNullOrEmpty(RequiredPolicy))
        {
            bool authorized;
            if (!string.IsNullOrEmpty(RequiredPolicyPermission))
            {
                // auth-policy="foo" & user is authorized for policy "foo" based on permission
                var cacheKey = AspAuthzPolicyPermissionAttributeName + "." + RequiredPolicyPermission;
                var cachedResult = ViewContext.ViewData[cacheKey];
                if (cachedResult != null)
                {
                    authorized = (bool)cachedResult;
                }
                else
                {
                    var authResult = await _authz.AuthorizeAsync(user, RequiredPolicyPermission, RequiredPolicy);
                    authorized = authResult.Succeeded;
                    ViewContext.ViewData[cacheKey] = authorized;
                }
            }
            else
            {
                // auth-policy="foo" & user is authorized for policy "foo"
                var cacheKey = AspAuthzPolicyAttributeName + "." + RequiredPolicy;
                var cachedResult = ViewContext.ViewData[cacheKey];
                if (cachedResult != null)
                {
                    authorized = (bool)cachedResult;
                }
                else
                {
                    var authResult = await _authz.AuthorizeAsync(user, ViewContext, RequiredPolicy);
                    authorized = authResult.Succeeded;
                    ViewContext.ViewData[cacheKey] = authorized;
                }
            }
            showOutput = authorized;
        }
        else if (!string.IsNullOrEmpty(RequiredRole))
        {
            showOutput = user.IsInRole(RequiredRole);
        }
        else if (requiresAuth && user.Identity.IsAuthenticated)
        {
            // auth="true" & user is authenticated
            showOutput = true;
        }

        if (!showOutput)
        {
            output.SuppressOutput();
            context.Items[SuppressedKey] = SuppressedValue;
        }
    }
}

/// <summary>
/// Extension methods for <see cref="TagHelperContext"/>.
/// </summary>
public static class AuthzTagHelperContextExtensions
{
    /// <summary>
    /// Determines if the <see cref="AuthzTagHelper"/> (<c>asp-authz</c>) has suppressed rendering for the element associated with
    /// this <see cref="TagHelperContext"/>.
    /// </summary>
    /// <param name="context">The <see cref="TagHelperContext"/>.</param>
    /// <returns><c>true</c> if <c>asp-authz</c> suppressed rendering of this Tag Helper.</returns>
    public static bool SuppressedByAspAuthz(this TagHelperContext context) =>
        context.Items.TryGetValue(AuthzTagHelper.SuppressedKey, out var value) && value == AuthzTagHelper.SuppressedValue;
}
