namespace ContentRatingAPI.Application.ContentPartyEstimationRoom.StartContentPartyEstimation
{
    public record StartContentPartyEstimationCommand(Guid RoomId, Guid ContentListId, string Name, double MinRating, double MaxRating,
        Guid CreatorId, string CreatorName) : IRequest;
}
