using MediatR;

namespace ContentRating.Domain.AggregatesModel.ContentRoomEditorAggregate.Events
{
    public record EvaluationCompletedDomainEvent(Guid RoomId, IReadOnlyCollection<Editor> InvitedUsers) : INotification;
}
