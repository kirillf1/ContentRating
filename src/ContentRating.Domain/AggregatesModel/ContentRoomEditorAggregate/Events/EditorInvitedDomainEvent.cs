

namespace ContentRating.Domain.AggregatesModel.ContentRoomEditorAggregate.Events
{
    public record EditorInvitedDomainEvent(Guid RoomId, Editor Inviter, Editor NewEditor) : INotification;
}
