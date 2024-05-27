using MediatR;

namespace ContentRating.Domain.AggregatesModel.ContentRoomEditorAggregate.Events
{
    public record EvaluationStartedDomainEvent(Guid RoomId, Editor Creator, 
        IReadOnlyCollection<Editor> InvitedEditors, IReadOnlyCollection<Content> AddedContent, Rating MinRating, Rating MaxRating) : INotification;

}
