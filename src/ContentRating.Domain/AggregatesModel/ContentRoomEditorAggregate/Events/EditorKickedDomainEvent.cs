

namespace ContentRating.Domain.AggregatesModel.ContentRoomEditorAggregate.Events
{
    public record EditorKickedDomainEvent(Guid RoomId, Editor Initiator, Editor KickedEditor) : INotification;
}
