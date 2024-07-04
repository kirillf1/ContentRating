using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ContentRatingAPI.Infrastructure.Authorization
{
    public static class AuthorizationExtensions
    {
        public static IServiceCollection AddApplicationAuthorization(this IHostApplicationBuilder builder)
        {
            builder.Services.AddScoped<IUserInfoService, UserInfoService>();
            builder.Services.AddScoped<IAuthorizationHandler, ContentRoomEditorUserAccessHandler>();
            builder.Services.AddScoped<IAuthorizationHandler, ContentPartyEstimationRoomRaterAccessHandler>();
            builder.Services.AddAuthorization(options =>
            {
                var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(
                    JwtBearerDefaults.AuthenticationScheme);
                defaultAuthorizationPolicyBuilder =
                    defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();
                options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
            });
            builder.Services.AddAuthorizationBuilder()
                .AddPolicy(Policies.ContentRoomEditorUserAccessPolicyName, policy =>
                {
                    policy.Requirements.Add(new ContentRoomEditorUserAccessRequirement());
                    policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                }).AddPolicy(Policies.ContentEstimationRoomUserAccessPolicyName, policy =>
                {
                    policy.Requirements.Add(new ContentPartyEstimationRoomRaterAccessRequirement());
                    policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                });

            return builder.Services;
        }
        static List<string> roomNameOptions = ["roomid", "room", "room-id", "room_id"];
        internal static Guid? TryGetRoomIdFromHttpContext(this HttpContext httpContext)
        {
            try
            {
                var roomIdFromRoute = httpContext.GetRouteData().Values
                    .FirstOrDefault(c => roomNameOptions.Contains(c.Key, StringComparer.OrdinalIgnoreCase)).Value?.ToString();
                if (Guid.TryParse(roomIdFromRoute, out var roomId))
                    return roomId;

                var roomIdFromQuery = httpContext.Request.Query
                     .FirstOrDefault(c => roomNameOptions.Contains(c.Key, StringComparer.OrdinalIgnoreCase)).Value;
                if (Guid.TryParse(roomIdFromQuery, out roomId))
                    return roomId;

                return null;
            }
            catch
            {
                return null;
            }
        }
        public static string? GetUserId(this ClaimsPrincipal principal)
            => principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        public static string? GetUserName(this ClaimsPrincipal principal) =>
            principal.FindFirst(x => x.Type == JwtRegisteredClaimNames.Name)?.Value;
        public static string? GetUserEmail(this ClaimsPrincipal principal) =>
            principal.FindFirst(x => x.Type == JwtRegisteredClaimNames.Email)?.Value;
    }
}
