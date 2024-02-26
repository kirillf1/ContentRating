using MediatR;

namespace ContentRating.Domain.AggregatesModel.ContentRoomAggregate.Events
{
    public record ContentUpdatedInRoomDomainEvent(Content UpdatedContent, Guid RoomId) : INotification;
}
