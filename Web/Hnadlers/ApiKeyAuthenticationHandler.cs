using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Brownsquare_twilio_backend.Hnadlers
{
    public class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string SchemeName = "ApiKey";

        //Constructor
        public ApiKeyAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder)
            : base(options, logger, encoder)
        { }

        //Validar la autenticación
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            const string headerName = "X-API-KEY";

            if (!Request.Headers.TryGetValue(headerName, out var providedApiKey))
            {
                return Task.FromResult(AuthenticateResult.Fail("API Key faltante"));
            }

            var configuredApiKey = Context.RequestServices
                .GetRequiredService<IConfiguration>()["JWT:ApiKey:key"];

            if (configuredApiKey != providedApiKey)
            {
                return Task.FromResult(AuthenticateResult.Fail("API Key inválida"));
            }

            // Usuario autenticado
            var claims = new[]
            {
            new Claim(ClaimTypes.Name, "ApiKeyUser")
        };

            var identity = new ClaimsIdentity(claims, SchemeName);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, SchemeName);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
