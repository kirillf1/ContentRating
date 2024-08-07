namespace ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate
{
    public interface IContentPartyEstimationRoomRepository : IRepository<ContentPartyEstimationRoom>
    {
        ContentPartyEstimationRoom Add(ContentPartyEstimationRoom room);
        ContentPartyEstimationRoom Update(ContentPartyEstimationRoom room);
        void Delete(ContentPartyEstimationRoom room);
        Task<ContentPartyEstimationRoom?> GetRoom(Guid id);
        Task<bool> HasRaterInRoom(Guid roomId, Guid raterId);
    }
}
