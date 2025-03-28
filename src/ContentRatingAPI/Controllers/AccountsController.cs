﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security.Claims;
using Ardalis.Result.AspNetCore;
using ContentRatingAPI.Application.Identity;
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
            return userInfo is null ? (Result<IEnumerable<UserResponse>>)Result.Forbidden() : await mediator.Send(new GetAllUsersQuery(userInfo.Id));
        }

        [HttpGet("login-google")]
        public IActionResult Login()
        {
            var props = new AuthenticationProperties { RedirectUri = Url.Action(nameof(GoogleSignInCallback)) };
            return Challenge(props, GoogleDefaults.AuthenticationScheme);
        }

        [TranslateResultToActionResult()]
        [HttpPost("refresh-token")]
        public async Task<Result<LoginResult>> RefreshToken([FromBody] RefreshTokenRequest refreshTokenRequest, [FromServices] IMediator mediator)
        {
            var refreshTokenCommand = new RefreshTokenCommand(refreshTokenRequest.ExpiredAccessToken, refreshTokenRequest.RefreshToken);
            return await mediator.Send(refreshTokenCommand);
        }

        [TranslateResultToActionResult()]
        [HttpGet("signin-google")]
        public async Task<Result<LoginResult>> GoogleSignInCallback([FromServices] IMediator mediator)
        {
            var response = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var accessToken = await HttpContext.GetTokenAsync(CookieAuthenticationDefaults.AuthenticationScheme, "access_token");

            if (response.Principal == null)
            {
                return Result.Error();
            }

            var name = response.Principal.FindFirstValue(ClaimTypes.Name)!;

            var email = response.Principal.FindFirstValue(ClaimTypes.Email)!;
            var loginResult = await mediator.Send(new RegisterOrLoginOAuthUserCommand(name, email, GoogleDefaults.AuthenticationScheme, accessToken));

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return loginResult;
        }
    }
}
