using Ardalis.Result;
using ContentRatingAPI.Infrastructure.Authorization;
using Microsoft.AspNetCore.Identity;

namespace ContentRatingAPI.Application.Identity.RefreshToken
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<LoginResult>>
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IJwtProvider jwtProvider;

        public RefreshTokenCommandHandler(UserManager<ApplicationUser> userManager, IJwtProvider jwtProvider)
        {
            this.userManager = userManager;
            this.jwtProvider = jwtProvider;
        }

        public async Task<Result<LoginResult>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var principal = jwtProvider.GetPrincipalFromExpiredToken(request.AccessToken);

            var email = principal.GetUserEmail();
            if(email is null)
                return Result.Invalid(new ValidationError("Unknown email"));

            var user = await userManager.FindByEmailAsync(email);
            if(user is null || user.RefreshTokenExpirationDate <= DateTime.UtcNow || user.RefreshToken != request.RefreshToken)
                return Result.Invalid(new ValidationError("Invalid refresh token"));

            var newToken = await jwtProvider.Generate(user.Id, user.Email!, user.UserName!);
            user.RefreshToken = newToken.RefreshToken;
            await userManager.UpdateAsync(user);
            return new LoginResult(user.Id, newToken.Token, user.RefreshToken);
        }
    }
}
