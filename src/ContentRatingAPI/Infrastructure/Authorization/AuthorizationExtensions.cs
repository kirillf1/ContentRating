﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace ContentRatingAPI.Infrastructure.Authorization
{
    public static class AuthorizationExtensions
    {
        public static IServiceCollection AddApplicationAuthorization(this IHostApplicationBuilder builder)
        {
            builder.Services.AddScoped<IUserInfoService, UserInfoService>();
            builder.Services.AddScoped<IAuthorizationHandler, ContentEstimationListEditorUserAccessHandler>();
            builder.Services.AddScoped<IAuthorizationHandler, ContentPartyEstimationRoomRaterAccessHandler>();
            builder.Services.AddAuthorization(options =>
            {
                var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme);
                defaultAuthorizationPolicyBuilder = defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();
                options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
            });
            builder
                .Services.AddAuthorizationBuilder()
                .AddPolicy(
                    Policies.ContentEstimationListEditorUserAccessPolicyName,
                    policy =>
                    {
                        policy.Requirements.Add(new ContentEstimationListEditorUserAccessRequirement());
                        policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                    }
                )
                .AddPolicy(
                    Policies.ContentEstimationRoomUserAccessPolicyName,
                    policy =>
                    {
                        policy.Requirements.Add(new ContentPartyEstimationRoomRaterAccessRequirement());
                        policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                    }
                );

            return builder.Services;
        }

        static List<string> roomNameOptions = ["roomid", "room", "room-id", "room_id"];

        internal static Guid? TryGetRoomIdFromHttpContext(this HttpContext httpContext)
        {
            try
            {
                var roomIdFromRoute = httpContext
                    .GetRouteData()
                    .Values.FirstOrDefault(c => roomNameOptions.Contains(c.Key, StringComparer.OrdinalIgnoreCase))
                    .Value?.ToString();
                if (Guid.TryParse(roomIdFromRoute, out var roomId))
                {
                    return roomId;
                }

                var roomIdFromQuery = httpContext
                    .Request.Query.FirstOrDefault(c => roomNameOptions.Contains(c.Key, StringComparer.OrdinalIgnoreCase))
                    .Value;
                if (Guid.TryParse(roomIdFromQuery, out roomId))
                {
                    return roomId;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public static string? GetUserId(this ClaimsPrincipal principal) => principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        public static string? GetUserName(this ClaimsPrincipal principal) => principal.FindFirst(x => x.Type == JwtRegisteredClaimNames.Name)?.Value;

        public static string? GetUserEmail(this ClaimsPrincipal principal) =>
            principal.FindFirst(x => x.Type == JwtRegisteredClaimNames.Email)?.Value;
    }
}
