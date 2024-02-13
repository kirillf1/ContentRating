using MediatR;

namespace ContentRating.Domain.AggregatesModel.ContentRoomAggregate.Events
{
    public record ContentAddedToRoomDomainEvent(Content NewContent, Guid RoomId) : INotification;
    
}
