namespace ContentRatingAPI.Application.ContentPartyEstimationRoom.CreateContentPartyEstimation
{
    public record CreateContentPartyEstimationCommand(Guid RoomId, Guid ContentListId, string Name, double MinRating, double MaxRating, 
        Guid CreatorId, string CreatorName) : IRequest;
}
