namespace ContentRatingAPI.Infrastructure.Data
{
    public class MongoDBOptions
    {
        public const string Position = "MongoDB";

        public string Connection { get; set; } = String.Empty;
        public string DatabaseName { get; set; } = String.Empty;
    }
}
