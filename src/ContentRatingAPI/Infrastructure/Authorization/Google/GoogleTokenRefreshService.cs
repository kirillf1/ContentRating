// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json;
using ContentRatingAPI.Application.Identity;
using ContentRatingAPI.Infrastructure.YoutubeClient;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace ContentRatingAPI.Infrastructure.Authorization.Google
{
    public class GoogleTokenRefreshService
    {
        private readonly HttpClient httpClient;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IConfiguration configuration;
        private readonly ILogger<GoogleTokenRefreshService> logger;

        public GoogleTokenRefreshService(
            HttpClient httpClient,
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            ILogger<GoogleTokenRefreshService> logger
        )
        {
            this.httpClient = httpClient;
            this.userManager = userManager;
            this.configuration = configuration;
            this.logger = logger;
        }

        public async Task<string?> GetValidAccessTokenAsync(ApplicationUser user)
        {
            // Проверяем, нужно ли обновить токен
            if (IsTokenValid(user))
            {
                return user.ExternalResourceAccessToken;
            }

            if (string.IsNullOrEmpty(user.ExternalResourceRefreshToken))
            {
                logger.LogWarning(
                    "Refresh token is missing for user {UserId}. User needs to re-authenticate with Google.",
                    user.Id
                );
                return null;
            }

            try
            {
                var newTokens = await RefreshAccessTokenAsync(user.ExternalResourceRefreshToken);
                if (newTokens != null)
                {
                    user.ExternalResourceAccessToken = newTokens.AccessToken;
                    user.ExternalResourceTokenExpiresAt = DateTime.UtcNow.AddSeconds(
                        newTokens.ExpiresIn
                    );

                    if (!string.IsNullOrEmpty(newTokens.RefreshToken))
                    {
                        user.ExternalResourceRefreshToken = newTokens.RefreshToken;
                    }

                    await userManager.UpdateAsync(user);
                    logger.LogInformation(
                        "Successfully refreshed access token for user {UserId}",
                        user.Id
                    );

                    return newTokens.AccessToken;
                }
                else
                {
                    logger.LogWarning(
                        "Failed to refresh token for user {UserId}. User may need to re-authenticate with Google.",
                        user.Id
                    );
                }
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Failed to refresh access token for user {UserId}. User may need to re-authenticate with Google.",
                    user.Id
                );
            }

            return null;
        }

        public bool HasValidRefreshToken(ApplicationUser user)
        {
            return !string.IsNullOrEmpty(user.ExternalResourceRefreshToken);
        }

        private static bool IsTokenValid(ApplicationUser user)
        {
            return !string.IsNullOrEmpty(user.ExternalResourceAccessToken)
                && user.ExternalResourceTokenExpiresAt.HasValue
                && user.ExternalResourceTokenExpiresAt.Value > DateTime.UtcNow.AddMinutes(5);
        }

        private async Task<GoogleTokenResponse?> RefreshAccessTokenAsync(string refreshToken)
        {
            var clientId = configuration["Authentication:Google:ClientId"];
            var clientSecret = configuration["Authentication:Google:ClientSecret"];

            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
            {
                logger.LogError("Google OAuth configuration is missing");
                return null;
            }

            var requestContent = new FormUrlEncodedContent(
                new[]
                {
                    new KeyValuePair<string, string>("client_id", clientId),
                    new KeyValuePair<string, string>("client_secret", clientSecret),
                    new KeyValuePair<string, string>("refresh_token", refreshToken),
                    new KeyValuePair<string, string>("grant_type", "refresh_token"),
                }
            );

            var response = await httpClient.PostAsync(
                "https://oauth2.googleapis.com/token",
                requestContent
            );

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                logger.LogError(
                    "Failed to refresh token. Status: {StatusCode}, Content: {Content}",
                    response.StatusCode,
                    errorContent
                );
                return null;
            }

            var jsonContent = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<GoogleTokenResponse>(
                jsonContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            return tokenResponse;
        }
    }
}
