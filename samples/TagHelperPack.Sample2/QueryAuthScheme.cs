using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TagHelperPack.Sample2;

public class QueryAuthScheme : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public QueryAuthScheme(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder urlEncoder, ISystemClock clock)
        : base(options, logger, urlEncoder, clock)
    {

    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var authQuery = Context.Request.Query["auth"];
        if (authQuery.Count == 0)
        {
            return Task.FromResult(AuthenticateResult.Fail("No auth type provided in query string"));
        }

        var identity = new ClaimsIdentity("QueryAuth");
        if (authQuery == "admin")
        {
            identity.AddClaim(new Claim("Name", "AdminUser"));
            identity.AddClaim(new Claim("IsAdmin", "true"));
        }
        else
        {
            identity.AddClaim(new Claim("Name", "StandardUser"));
        }
        var user = new ClaimsPrincipal(identity);
        return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(user, nameof(QueryAuthScheme))));
    }
}