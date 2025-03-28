﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security.Claims;

namespace ContentRatingAPI.Application.Identity
{
    public record TokenResponse(string Token, string RefreshToken);

    public interface IJwtProvider
    {
        Task<TokenResponse> Generate(Guid userId, string email, string name);
        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
