using Microsoft.AspNetCore.Identity;
using MongoDB.Driver.Linq;
using MongoDB.Driver;

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
            if(request.IgnoreUserId.HasValue)
                query = query.Where(c=>c.Id != request.IgnoreUserId.Value);
            return await query.Select(c => new UserResponse(c.Id, c.UserName!))
                .ToListAsync(cancellationToken: cancellationToken);
        }
    }
}
