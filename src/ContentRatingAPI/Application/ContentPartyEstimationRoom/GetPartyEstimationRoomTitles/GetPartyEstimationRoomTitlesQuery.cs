namespace ContentRatingAPI.Application.ContentPartyEstimationRoom.GetPartyEstimationRoomTitles
{
    public record class GetPartyEstimationRoomTitlesQuery(bool IncludeEstimated, 
        bool IncludeNotEstimated, Guid RelatedWithRaterId) : IRequest<Result<IEnumerable<PartyEstimationTitle>>>;
    
}
