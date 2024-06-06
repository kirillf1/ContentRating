namespace ContentRating.Domain.AggregatesModel.ContentPartyRatingRoomAggregate
{
    public interface IRoomAccessControlRepository : IRepository<ContentPartyRatingRoom>
    {
        ContentPartyRatingRoom Add(ContentPartyRatingRoom room);
        ContentPartyRatingRoom Update(ContentPartyRatingRoom room);
        void Delete(ContentPartyRatingRoom room);
        Task<ContentPartyRatingRoom> GetRoom(Guid id);

    }
}
