namespace ContentRatingAPI.Infrastructure.Authorization
{
    public interface IUserInfoService
    {
        public UserInfo? TryGetUserInfo();
    }
}
