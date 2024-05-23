using MediatR;

namespace ContentRating.Domain.AggregatesModel.ContentRoomEditorAggregate.Events
{
    public record EvaluationStartedDomainEvent(Guid RoomId, Editor Creator, IReadOnlyCollection<Editor> InvitedEditors) : INotification;

}
