using MediatR;

namespace ContentRating.Domain.AggregatesModel.RoomEditorAggregate.Events
{
    public record RoomDeletedDomainEvent(Guid RoomId) : INotification;
}
