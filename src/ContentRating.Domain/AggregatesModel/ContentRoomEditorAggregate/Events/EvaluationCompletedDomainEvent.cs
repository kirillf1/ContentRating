using MediatR;

namespace ContentRating.Domain.AggregatesModel.ContentRoomEditorAggregate.Events
{
    public record EvaluationCompletedDomainEvent(Guid RoomId, Editor Creator, IReadOnlyCollection<Editor> InvitedEditors) : INotification;
}
