using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace eZbori.Web.Security;

public class BasicAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory loggerFactory,
    UrlEncoder encoder,
    ISystemClock clock,
    IMediator mediator) : AuthenticationHandler<AuthenticationSchemeOptions>(options, loggerFactory, encoder, clock)
{
    private readonly IMediator _mediator = mediator;

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        //if (!Request.Headers.ContainsKey("Authorization"))
        //{
        //    return AuthenticateResult.Fail("Missing authorization handler!");
        //}

        //var claims = new List<Claim>();

        //try
        //{
        //    var authenticationHeader = AuthenticationHeaderValue.Parse(Request.Headers.Authorization);
        //    var credentialBytes = Convert.FromBase64String(authenticationHeader.Parameter);
        //    var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':');
        //    var username = credentials[0];
        //    var password = credentials[1];

        //    var user = await _mediator.Send(new UserWithRolesQuery(username, password));

        //    claims.Add(new Claim(ClaimTypes.Name, user.FirstName));
        //    claims.Add(new Claim(ClaimTypes.NameIdentifier, user.UserName));

        //    // TODO and refactor

        //    var roles = await _mediator.Send(new UserRolesQuery(user.Id));

        //    foreach (var role in roles.Select(x => x.Name.ToString()))
        //    {
        //        claims.Add(new Claim(ClaimTypes.Role, role));
        //    }
        //}
        //catch
        //{
        //    return AuthenticateResult.Fail("Incorrect username or password!");
        //}            

        //var identity = new ClaimsIdentity(claims, Scheme.Name);
        //var principal = new ClaimsPrincipal(identity);
        //var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return AuthenticateResult.Fail(new NotImplementedException());
    }
}
