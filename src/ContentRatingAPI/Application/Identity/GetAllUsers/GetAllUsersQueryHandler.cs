// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace ContentRatingAPI.Application.Identity.GetAllUsers
{
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, Result<IEnumerable<UserResponse>>>
    {
        private readonly UserManager<ApplicationUser> userManager;

        public GetAllUsersQueryHandler(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        public async Task<Result<IEnumerable<UserResponse>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var query = (IMongoQueryable<ApplicationUser>)userManager.Users;
            if (request.IgnoreUserId.HasValue)
            {
                query = query.Where(c => c.Id != request.IgnoreUserId.Value);
            }

            return await query.Select(c => new UserResponse(c.Id, c.UserName!)).ToListAsync(cancellationToken: cancellationToken);
        }
    }
}
