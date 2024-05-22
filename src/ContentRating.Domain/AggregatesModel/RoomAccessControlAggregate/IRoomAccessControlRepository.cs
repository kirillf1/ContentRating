using ContentRating.Domain.Shared;


namespace ContentRating.Domain.AggregatesModel.RoomAccessControlAggregate
{
    public interface IRoomAccessControlRepository : IRepository<RoomAccessControl>
    {
        RoomAccessControl Add(RoomAccessControl room);
        RoomAccessControl Update(RoomAccessControl room);
        void Delete(RoomAccessControl room);
        Task<RoomAccessControl> GetRoom(Guid id);

    }
}
