using ContentRating.Domain.AggregatesModel.ContentRoomEditorAggregate;
using MediatR;

namespace ContentRating.Domain.AggregatesModel.ContentRoomEditorAggregate.Events
{
    public record ContentUpdatedInRoomDomainEvent(Content UpdatedContent, Guid RoomId) : INotification;
}
