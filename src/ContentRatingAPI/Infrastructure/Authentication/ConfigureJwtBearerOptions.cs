using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace ContentRatingAPI.Infrastructure.Authentication
{
    public class ConfigureJwtBearerOptions : IConfigureNamedOptions<JwtBearerOptions>
    {
        private readonly RSA rsa;
        private readonly IOptions<JwtOptions> jwtOptions;

        public ConfigureJwtBearerOptions(RSA rsa, IOptions<JwtOptions> options)
        {
            this.rsa = rsa;
            this.jwtOptions = options;
        }

        public void Configure(JwtBearerOptions options)
        {
            var key = new RsaSecurityKey(rsa);
            
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = false,
                ValidateLifetime = true,
                ValidIssuer = jwtOptions.Value.Issuer,
                ValidAudience = jwtOptions.Value.Audience,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero,
                IssuerSigningKey = key
            };
            options.MapInboundClaims = false;
            options.SaveToken = true;
        }

        public void Configure(string name, JwtBearerOptions options)
        {
            Configure(options);
        }
    }
}
