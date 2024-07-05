using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ContentRatingAPI.Application.Identity.RegisterUser;
using ContentRatingAPI.Application.Identity.RefreshToken;
using ContentRatingAPI.Application.Identity;
using Ardalis.Result.AspNetCore;

namespace ContentRatingAPI.Controllers
{

    [Route("accounts")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
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
                return Result.Error();

            var name = response.Principal.FindFirstValue(ClaimTypes.Name)!;

            var email = response.Principal.FindFirstValue(ClaimTypes.Email)!;
            var loginResult = await mediator.Send(new RegisterOrLoginOAuthUserCommand(name, email, GoogleDefaults.AuthenticationScheme, accessToken));

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return loginResult;

        }
    }
}
