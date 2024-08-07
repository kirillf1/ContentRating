namespace ContentRatingAPI.Application.ContentPartyEstimationRoom.ChangeRatingRange
{
    public class ChangeRatingRangeRequest
    {
        public required double MaxRating { get; set; }
        public required double MinRating { get; set; }
    }
}
