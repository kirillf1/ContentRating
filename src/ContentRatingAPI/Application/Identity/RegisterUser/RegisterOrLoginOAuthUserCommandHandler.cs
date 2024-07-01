using Microsoft.AspNetCore.Identity;

namespace ContentRatingAPI.Application.Identity.RegisterUser
{
    public class RegisterOrLoginOAuthUserCommandHandler : IRequestHandler<RegisterOrLoginOAuthUserCommand, LoginResult>
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IJwtProvider jwtProvider;

        public RegisterOrLoginOAuthUserCommandHandler(UserManager<ApplicationUser> userManager, IJwtProvider jwtProvider)
        {
            this.userManager = userManager;
            this.jwtProvider = jwtProvider;
        }
        public async Task<LoginResult> Handle(RegisterOrLoginOAuthUserCommand request, CancellationToken cancellationToken)
        {
            Guid userId;
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user is null)
                userId = Guid.NewGuid();
            else userId = user.Id;

            var token = await jwtProvider.Generate(userId, request.Email, request.Name);
            if(user is null)
            {
                user = new ApplicationUser(userId, token.RefreshToken, request.AuthScheme, request.Email, request.Name)
                {
                    ExternalResourceAccessToken = request.AccessToken
                };
                var result = await userManager.CreateAsync(user);
                if (!result.Succeeded)
                    throw new ArgumentException($"Invalid user. Errors: {string.Join(", ", result.Errors.Select(c => c.Description))}");
            }
            else
            {
                if(!string.IsNullOrEmpty(request.AccessToken))
                    user.ExternalResourceAccessToken = request.AccessToken;
                user.AuthenticationScheme = request.AuthScheme;
                user.RefreshToken = token.RefreshToken;
                user.RefreshTokenExpirationDate = DateTime.UtcNow.AddDays(30);
                await userManager.UpdateAsync(user);
            }
            return new LoginResult(user.Id, token.Token, token.RefreshToken);
        }
    }
}
