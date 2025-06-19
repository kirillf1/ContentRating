// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace ContentRating.Web.Contracts.Identity
{
    public class RefreshTokenRequest
    {
        public required string ExpiredAccessToken { get; set; }
        public required string RefreshToken { get; set; }
    }
} 