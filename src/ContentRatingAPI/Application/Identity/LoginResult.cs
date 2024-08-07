namespace ContentRatingAPI.Application.Identity
{
    public record LoginResult(Guid UserId, string Token, string RefreshToken);

}
