namespace ContentRatingAPI.Application.ContentPartyEstimationRoom.GetPartyEstimationRoom
{
    public record class GetPartyEstimationRoomQuery(Guid RoomId) : IRequest<Result<PartyEstimationRoomResponse>>;
}
