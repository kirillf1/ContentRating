using ContentRating.Domain.AggregatesModel.RoomEditorAggregate;
using MediatR;

namespace ContentRating.Domain.AggregatesModel.RoomEditorAggregate.Events
{
    public record UserInvitedDomainEvent(Editor User, Guid RoomId) : INotification;
}
