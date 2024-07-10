using ContentRatingAPI.Application.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace ContentRatingAPI.Infrastructure.Authentication
{
    public class JwtProvider : IJwtProvider
    {
        private readonly RSA Rsa;
        private readonly IOptions<JwtOptions> options;

        public JwtProvider(RSA Rsa, IOptions<JwtOptions> options)
        {
            this.Rsa = Rsa;
            this.options = options;
        }
        public Task<TokenResponse> Generate(Guid userId, string email, string name)
        {
            var claims = new Claim[]
            {
                new(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new(JwtRegisteredClaimNames.Email, email),
                new(JwtRegisteredClaimNames.Name, name)
            };
            var key = new RsaSecurityKey(Rsa);
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256);

            var token = new JwtSecurityToken(
                options.Value.Issuer,
                options.Value.Audience,
                claims,
                null,
                DateTime.UtcNow.AddSeconds(options.Value.TokenLiveTimeSeconds),
                signingCredentials);

            var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

            return Task.FromResult(new TokenResponse(tokenValue, GenerateRefreshToken()));
        }
        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var key = new RsaSecurityKey(Rsa);
           
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateLifetime = false,
            };
            var tokenHandler = new JwtSecurityTokenHandler()
            {
                MapInboundClaims = false,
            };
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.RsaSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");
            return principal;
        }
        private static string GenerateRefreshToken()
        {
            var randomNumbers = new byte[64];
            using var numberGenerator = RandomNumberGenerator.Create();
            numberGenerator.GetBytes(randomNumbers);
            return Convert.ToBase64String(randomNumbers);
        }

    }
}
