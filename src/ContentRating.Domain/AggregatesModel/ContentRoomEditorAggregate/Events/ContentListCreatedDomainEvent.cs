

namespace ContentRating.Domain.AggregatesModel.ContentRoomEditorAggregate.Events
{
    public record ContentListCreatedDomainEvent(Guid RoomId, Editor Creator, 
        IReadOnlyCollection<Editor> InvitedEditors, IReadOnlyCollection<Content> AddedContent, string RoomName) : INotification;

}
