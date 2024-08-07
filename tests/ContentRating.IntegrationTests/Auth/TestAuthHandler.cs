using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace ContentRating.IntegrationTests.Auth
{
    public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string SchemeName = "IntegrationTests";
        public const string IDENTITY_ID = "fb252f79-1728-49f4-b6a0-b211719e4be1";
        public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger, UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var identity = new ClaimsIdentity("Test");
            identity.AddClaim(new Claim("sub", IDENTITY_ID));
            identity.AddClaim(new Claim("name", IDENTITY_ID));
            identity.AddClaim(new Claim("email", "testmail@gmail.com"));
            identity.AddClaim(new Claim("exp", TimeProvider.System.GetUtcNow().AddDays(1).ToUnixTimeSeconds().ToString()));

            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, JwtBearerDefaults.AuthenticationScheme);

            var result = AuthenticateResult.Success(ticket);

            return Task.FromResult(result);
        }
    }
}
