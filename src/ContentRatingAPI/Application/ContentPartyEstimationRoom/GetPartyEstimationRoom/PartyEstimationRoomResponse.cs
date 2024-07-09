namespace ContentRatingAPI.Application.ContentPartyEstimationRoom.GetPartyEstimationRoom
{
    public class PartyEstimationRoomResponse
    {
        
        public PartyEstimationRoomResponse(Guid id, string Name, double minRating, double maxRating, string creatorName,
            IEnumerable<ContentRatingResponse> contentRatings, IEnumerable<RaterResponse> raters)
        {
            Id = id;
            this.Name = Name;
            MinRating = minRating;
            MaxRating = maxRating;
            CreatorName = creatorName;
            ContentRatings = contentRatings;
            Raters = raters;
        }

        public Guid Id { get; }
        public string Name { get; }
        public double MinRating { get; }
        public double MaxRating { get; }
        public string CreatorName { get; }
        public IEnumerable<ContentRatingResponse> ContentRatings { get; }
        public IEnumerable<RaterResponse> Raters { get; }
    }
   
}
