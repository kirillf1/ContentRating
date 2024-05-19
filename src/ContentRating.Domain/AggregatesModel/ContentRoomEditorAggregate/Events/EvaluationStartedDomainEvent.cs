using MediatR;

namespace ContentRating.Domain.AggregatesModel.ContentRoomEditorAggregate.Events
{
    public record EvaluationStartedDomainEvent(Guid RoomId, IReadOnlyCollection<Editor> InvitedUsers) : INotification;

}
