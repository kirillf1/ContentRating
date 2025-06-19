// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ContentRating.Web.Contracts.Identity;

namespace ContentRatingAPI.Application.Identity.GetAllUsers
{
    public record class GetAllUsersQuery(Guid? IgnoreUserId = null) : IRequest<Result<IEnumerable<UserResponse>>>;
}
