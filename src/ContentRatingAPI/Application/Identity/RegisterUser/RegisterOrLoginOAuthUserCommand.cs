// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace ContentRatingAPI.Application.Identity.RegisterUser
{
    public record RegisterOrLoginOAuthUserCommand(string Name, string Email, string AuthScheme, string? AccessToken) : IRequest<Result<LoginResult>>;
}
