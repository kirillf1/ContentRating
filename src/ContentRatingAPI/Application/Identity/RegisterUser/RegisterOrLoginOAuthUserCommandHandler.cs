// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ContentRating.Web.Contracts.Identity;
using Microsoft.AspNetCore.Identity;

namespace ContentRatingAPI.Application.Identity.RegisterUser
{
    public class RegisterOrLoginOAuthUserCommandHandler : IRequestHandler<RegisterOrLoginOAuthUserCommand, Result<LoginResult>>
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IJwtProvider jwtProvider;

        public RegisterOrLoginOAuthUserCommandHandler(UserManager<ApplicationUser> userManager, IJwtProvider jwtProvider)
        {
            this.userManager = userManager;
            this.jwtProvider = jwtProvider;
        }

        public async Task<Result<LoginResult>> Handle(RegisterOrLoginOAuthUserCommand request, CancellationToken cancellationToken)
        {
            Guid userId;
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user is null)
            {
                userId = Guid.NewGuid();
            }
            else
            {
                userId = user.Id;
            }

            var token = await jwtProvider.Generate(userId, request.Email, request.Name);
            if (user is null)
            {
                user = new ApplicationUser(userId, token.RefreshToken, request.AuthScheme, request.Email, request.Name)
                {
                    ExternalResourceAccessToken = request.AccessToken,
                    ExternalResourceRefreshToken = request.RefreshToken,
                    ExternalResourceTokenExpiresAt = ParseExpiresAt(request.ExpiresAt)
                };
                var result = await userManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    throw new ArgumentException($"Invalid user. Errors: {string.Join(", ", result.Errors.Select(c => c.Description))}");
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(request.AccessToken))
                {
                    user.ExternalResourceAccessToken = request.AccessToken;
                }

                if (!string.IsNullOrEmpty(request.RefreshToken))
                {
                    user.ExternalResourceRefreshToken = request.RefreshToken;
                }

                user.ExternalResourceTokenExpiresAt = ParseExpiresAt(request.ExpiresAt);
                user.AuthenticationScheme = request.AuthScheme;
                user.RefreshToken = token.RefreshToken;
                user.RefreshTokenExpirationDate = DateTime.UtcNow.AddDays(30);
                await userManager.UpdateAsync(user);
            }
            return new LoginResult(user.Id, token.Token, token.RefreshToken);
        }

        private static DateTime? ParseExpiresAt(string? expiresAt)
        {
            if (string.IsNullOrEmpty(expiresAt))
                return null;

            if (DateTimeOffset.TryParse(expiresAt, out var dateTimeOffset))
            {
                return dateTimeOffset.UtcDateTime;
            }

            return null;
        }
    }
}
