// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security.Claims;
using Ardalis.Result.AspNetCore;
using ContentRating.Web.Contracts.Identity;
using ContentRatingAPI.Application.Identity.GetAllUsers;
using ContentRatingAPI.Application.Identity.RefreshToken;
using ContentRatingAPI.Application.Identity.RegisterUser;
using ContentRatingAPI.Infrastructure.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContentRatingAPI.Controllers
{
    [Route("accounts")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        [Authorize]
        [TranslateResultToActionResult]
        [HttpGet]
        public async Task<Result<IEnumerable<UserResponse>>> GetUsers(
            [FromServices] IMediator mediator,
            [FromServices] IUserInfoService userInfoService
        )
        {
            var userInfo = userInfoService.TryGetUserInfo();
            return userInfo is null
                ? (Result<IEnumerable<UserResponse>>)Result.Forbidden()
                : await mediator.Send(new GetAllUsersQuery(userInfo.Id));
        }

        [HttpGet("login-google")]
        public IActionResult Login([FromQuery] string? returnUrl = null)
        {
            var props = new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(GoogleSignInCallback), new { returnUrl }),
            };
            return Challenge(props, GoogleDefaults.AuthenticationScheme);
        }

        [TranslateResultToActionResult()]
        [HttpPost("refresh-token")]
        public async Task<Result<LoginResult>> RefreshToken(
            [FromBody] RefreshTokenRequest refreshTokenRequest,
            [FromServices] IMediator mediator
        )
        {
            var refreshTokenCommand = new RefreshTokenCommand(
                refreshTokenRequest.ExpiredAccessToken,
                refreshTokenRequest.RefreshToken
            );
            return await mediator.Send(refreshTokenCommand);
        }

        [HttpGet("signin-google")]
        public async Task<IActionResult> GoogleSignInCallback(
            [FromServices] IMediator mediator,
            [FromServices] ILogger<AccountsController> logger,
            [FromQuery] string? returnUrl = null
        )
        {
            var response = await HttpContext.AuthenticateAsync(
                CookieAuthenticationDefaults.AuthenticationScheme
            );
            var accessToken = await HttpContext.GetTokenAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                "access_token"
            );
            var refreshToken = await HttpContext.GetTokenAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                "refresh_token"
            );
            var expiresAt = await HttpContext.GetTokenAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                "expires_at"
            );

            if (response.Principal == null)
            {
                var errorUrl = BuildCallbackUrl(returnUrl, error: "authentication_failed");
                return Redirect(errorUrl);
            }

            var name = response.Principal.FindFirstValue(ClaimTypes.Name)!;
            var email = response.Principal.FindFirstValue(ClaimTypes.Email)!;

            var loginResult = await mediator.Send(
                new RegisterOrLoginOAuthUserCommand(
                    name,
                    email,
                    GoogleDefaults.AuthenticationScheme,
                    accessToken,
                    refreshToken,
                    expiresAt
                )
            );

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (loginResult.IsSuccess)
            {
                var successUrl = BuildCallbackUrl(
                    returnUrl,
                    loginResult.Value.Token,
                    loginResult.Value.RefreshToken
                );
                return Redirect(successUrl);
            }
            else
            {
                var errorUrl = BuildCallbackUrl(returnUrl, error: "login_failed");
                return Redirect(errorUrl);
            }
        }

        private string BuildCallbackUrl(
            string? returnUrl,
            string? token = null,
            string? refreshToken = null,
            string? error = null
        )
        {
            var baseUrl = returnUrl ?? "http://localhost:5173/login-callback";
            var queryParams = new List<string>();

            if (!string.IsNullOrEmpty(token))
                queryParams.Add($"token={Uri.EscapeDataString(token)}");

            if (!string.IsNullOrEmpty(refreshToken))
                queryParams.Add($"refreshToken={Uri.EscapeDataString(refreshToken)}");

            if (!string.IsNullOrEmpty(error))
                queryParams.Add($"error={Uri.EscapeDataString(error)}");

            if (queryParams.Count > 0)
                baseUrl += "?" + string.Join("&", queryParams);

            return baseUrl;
        }
    }
}
