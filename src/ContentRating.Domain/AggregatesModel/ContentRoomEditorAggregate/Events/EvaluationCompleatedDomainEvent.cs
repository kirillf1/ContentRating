using MediatR;

namespace ContentRating.Domain.AggregatesModel.ContentRoomEditorAggregate.Events
{
    public record EvaluationCompleatedDomainEvent(Guid RoomId, IReadOnlyCollection<Editor> InvitedUsers) : INotification;
}
