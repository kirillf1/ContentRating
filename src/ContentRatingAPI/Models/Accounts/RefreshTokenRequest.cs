namespace ContentRatingAPI.Models.Authentication
{
    public class RefreshTokenRequest
    {
        public string ExpiredAccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
