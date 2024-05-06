using ContentRating.Domain.Shared;

namespace ContentRating.Domain.AggregatesModel.RoomEditorAggregate
{
    public interface IRoomRepository : IRepository<RoomEditor>
    {
        RoomEditor Add(RoomEditor room);
        RoomEditor Update(RoomEditor room);
        void Delete(RoomEditor room);
        Task<RoomEditor> GetRoom(Guid id);

    }
}
