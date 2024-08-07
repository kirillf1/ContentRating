namespace ContentRatingAPI.Application.Identity.RefreshToken
{
    public record RefreshTokenCommand(string AccessToken, string RefreshToken) : IRequest<Result<LoginResult>>;
}
