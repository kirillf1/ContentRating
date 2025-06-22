namespace ContentRating.Web.UI.Services.Authentication
{
    public record UserData(string UserId, string UserName, string UserEmail, DateTime TokenExpiry);
}
