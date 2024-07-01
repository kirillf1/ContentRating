namespace ContentRatingAPI.Infrastructure.Authentication
{
    public class JwtOptions
    {
        public string Issuer { get; init; } = string.Empty;

        public string Audience { get; init; } = string.Empty;
        public int TokenLiveTimeSeconds { get; init; } = 300;
    }
}
