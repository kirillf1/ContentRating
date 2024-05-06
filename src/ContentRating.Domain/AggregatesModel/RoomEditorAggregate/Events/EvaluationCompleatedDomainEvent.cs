using ContentRating.Domain.AggregatesModel.RoomEditorAggregate;
using MediatR;

namespace ContentRating.Domain.AggregatesModel.RoomEditorAggregate.Events
{
    public record EvaluationCompleatedDomainEvent(Guid RoomId, IReadOnlyCollection<Editor> InvitedUsers) : INotification;
}
