using AspNetCore.Identity.Mongo.Model;

namespace ContentRatingAPI.Application.Identity
{
    public class ApplicationUser : MongoUser<Guid>
    {
        public string RefreshToken { get; set; }
        public string? ExternalResourceAccessToken { get; set; }
        public string AuthenticationScheme { get; set; }
        public DateTime RefreshTokenExpirationDate { get; set; }
        public ApplicationUser(Guid id, string refreshToken, string authenticationScheme, string email, string userName)
        {
            RefreshToken = refreshToken;
            AuthenticationScheme = authenticationScheme;
            Id = id;
            Email = email;
            UserName = userName;
            RefreshTokenExpirationDate = DateTime.UtcNow.AddDays(30);
        }

    }
}
