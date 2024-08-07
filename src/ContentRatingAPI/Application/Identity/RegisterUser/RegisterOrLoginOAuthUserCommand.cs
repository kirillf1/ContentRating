namespace ContentRatingAPI.Application.Identity.RegisterUser
{
    public record RegisterOrLoginOAuthUserCommand(string Name, string Email, string AuthScheme, string? AccessToken) : IRequest<Result<LoginResult>>;

}
