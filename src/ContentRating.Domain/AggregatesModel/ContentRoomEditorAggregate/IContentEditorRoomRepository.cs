namespace ContentRating.Domain.AggregatesModel.ContentRoomEditorAggregate
{
    public interface IContentEditorRoomRepository : IRepository<ContentRoomEditor>
    {
        ContentRoomEditor Add(ContentRoomEditor room);
        ContentRoomEditor Update(ContentRoomEditor room);
        void Delete(ContentRoomEditor room);
        Task<ContentRoomEditor?> GetRoom(Guid id);
        Task<bool> HasEditorInRoom(Guid roomId, Guid editorId);
    }
}
