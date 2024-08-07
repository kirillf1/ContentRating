using System.Security.Claims;

namespace ContentRatingAPI.Application.Identity
{
    public record TokenResponse(string Token, string RefreshToken);
    public interface IJwtProvider
    {
        Task<TokenResponse> Generate(Guid userId, string email, string name);
        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
