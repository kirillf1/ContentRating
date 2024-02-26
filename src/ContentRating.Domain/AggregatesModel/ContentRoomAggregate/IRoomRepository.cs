using ContentRating.Domain.Shared;

namespace ContentRating.Domain.AggregatesModel.ContentRoomAggregate
{
    public interface IRoomRepository : IRepository<Room>
    {
        Room Add(Room room);
        Room Update(Room room);
        void Delete(Room room);
        Task<Room> GetRoom(Guid id);
        
    }
}
