// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security.Claims;

namespace ContentRatingAPI.Infrastructure.Authorization
{
    public class UserInfoService : IUserInfoService
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public UserInfoService(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public UserInfo? TryGetUserInfo()
        {
            if (httpContextAccessor.HttpContext is null)
            {
                return null;
            }

            var user = httpContextAccessor.HttpContext.User;

            return GetUserInfoFromClaimsPrincipal(user);
        }

        public UserInfo? TryGetUserInfo(ClaimsPrincipal? claims)
        {
            if (claims is null)
            {
                return TryGetUserInfo();
            }

            return GetUserInfoFromClaimsPrincipal(claims);
        }

        private static UserInfo? GetUserInfoFromClaimsPrincipal(ClaimsPrincipal user)
        {
            if (!Guid.TryParse(user.GetUserId(), out var userId))
            {
                return null;
            }

            var name = user.GetUserName();
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            var email = user.GetUserEmail();
            if (string.IsNullOrEmpty(email))
            {
                return null;
            }

            return new UserInfo(userId, name, email);
        }
    }
}
