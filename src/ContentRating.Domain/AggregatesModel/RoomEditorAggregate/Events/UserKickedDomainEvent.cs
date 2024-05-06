using ContentRating.Domain.AggregatesModel.RoomEditorAggregate;
using MediatR;

namespace ContentRating.Domain.AggregatesModel.RoomEditorAggregate.Events
{
    public record UserKickedDomainEvent(Editor KickedUser, Guid RoomId) : INotification;
}
