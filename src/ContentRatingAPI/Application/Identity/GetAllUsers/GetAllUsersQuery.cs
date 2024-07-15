namespace ContentRatingAPI.Application.Identity.GetAllUsers
{
    public record class GetAllUsersQuery(Guid? IgnoreUserId = null) : IRequest<Result<IEnumerable<UserResponse>>>;    

}
