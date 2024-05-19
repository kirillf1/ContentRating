using ContentRating.Domain.Shared;

namespace ContentRating.Domain.AggregatesModel.ContentRoomEditorAggregate
{
    public interface IRoomRepository : IRepository<ContentRoomEditor>
    {
        ContentRoomEditor Add(ContentRoomEditor room);
        ContentRoomEditor Update(ContentRoomEditor room);
        void Delete(ContentRoomEditor room);
        Task<ContentRoomEditor> GetRoom(Guid id);

    }
}
