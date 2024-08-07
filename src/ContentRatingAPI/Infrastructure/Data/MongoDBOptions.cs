namespace ContentRatingAPI.Infrastructure.Data
{
    public class MongoDBOptions
    {
        public const string Position = "MongoDB";

        public string Connection { get; set; } = String.Empty;
        public string DatabaseName { get; set; } = String.Empty;
        public string ContentEstimationListEditorCollectionName { get; set; } = String.Empty;
        public string ContentPartyEstimationRoomCollectionName { get; set; } = String.Empty;
        public string ContentPartyRatingCollectionName {  get; set; } = String.Empty;
        public string SavedContentFileCollectionName { get; set; } = String.Empty;
    }
}
